using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Printer : MonoBehaviour
    {
        private Letter[] _letters;
        private Sequence _appearSequence;
        private TMP_Text _textComponent;
        private bool _isPlaying;
        private float _speed = 1f;

        // 모든 Printer 의 글자 등장 속도에 곱해지는 전역 배율 (1 = 기본, 2 = 두 배 빠르게).
        // 텍스트 속도 같은 사용자 설정을 호스트 프로젝트가 SetGlobalSpeedMultiplier 로 밀어 넣는다.
        public static float GlobalSpeedMultiplier { get; private set; } = 1f;
        public static event Action OnGlobalSpeedChanged;

        public static void SetGlobalSpeedMultiplier(float value)
        {
            if (value <= 0f)
            {
                Debug.LogWarning($"{nameof(Printer)}.{nameof(SetGlobalSpeedMultiplier)} value must be greater than 0 (was {value}). Using 1.");
                value = 1f;
            }

            GlobalSpeedMultiplier = value;
            OnGlobalSpeedChanged?.Invoke();
        }

        // LateUpdate 에서 매 프레임 수십 번 접근하므로 GetComponent 결과를 캐싱한다.
        // Awake 가 아니라 지연 초기화인 이유는 에디터에서 Setup 을 직접 호출하는
        // PrinterTester 같은 사용처가 있기 때문이다.
        private TMP_Text textComponent
        {
            get
            {
                if (_textComponent == null)
                {
                    _textComponent = GetComponent<TMP_Text>();
                }

                return _textComponent;
            }
        }

     
        public void Setup(string text, TMP_FontAsset font = null)
        {
            // 이전 Setup 이 만들어둔 것들을 먼저 정리한다.
            // appear 시퀀스는 SetAutoKill(false) 라 Kill 하지 않으면 DOTween 활성 목록에
            // 계속 남고, letter 의 반복 트윈은 SetLoops(-1) 이라 스스로 끝나지 않는다.
            // _letters 를 새로 만들기 전에 호출해야 예전 letter 의 반복 트윈까지 해제된다.
            Stop();

            if (font)
            {
                textComponent.font = font;
            }
            
            _letters = GenerateLetter(text);

            // _letters 는 스타일 태그가 제거된 글자들이다.
            // LateUpdate 가 characterCount 범위로 _letters 를 인덱싱하므로
            // 원본 text 가 아니라 여기서 재조립한 문자열을 넣어야 길이가 맞는다.
            var builder = new StringBuilder(_letters.Length);
            foreach (var letter in _letters)
            {
                builder.Append(letter.value);
            }

            textComponent.text = builder.ToString();

            _appearSequence = GenerateAppearSequence(_letters);
        }


        // speed 는 이번 출력의 속도 배율이다 (1 = PrintStyle 에 정의된 기본 속도). 실제 속도는 speed × GlobalSpeedMultiplier.
        // 글자 등장 연출(appear)에만 적용되고, 등장 이후의 반복 연출(repeat)은 영향을 받지 않는다.
        public Tween Print(float delay = 0, TweenCallback callback = null, float speed = 1f)
        {
            if (_appearSequence == null)
            {
                Debug.LogWarning($"{nameof(Printer)}.{nameof(Print)} called before {nameof(Setup)}.", this);
                return DOTween.Sequence().Play();
            }

            if (speed <= 0f)
            {
                Debug.LogWarning($"{nameof(Printer)}.{nameof(Print)} speed must be greater than 0 (was {speed}). Using 1.", this);
                speed = 1f;
            }

            _speed = speed;
            ApplyTimeScale();

            if (_isPlaying)
            {
                // 이미 재생 중이면 진행 중인 시퀀스를 그대로 돌려준다.
                // 호출부의 WaitForCompletion() 이 깨지지 않도록 null 을 반환하지 않는다.
                return _appearSequence;
            }

            _isPlaying = true;
            _appearSequence.OnComplete(() =>
            {
                _isPlaying = false;
                callback?.Invoke();
            });

            // DOTween 은 delay 도 timeScale 이 적용된 시간으로 소모하므로,
            // 실제 대기 시간이 속도와 무관하게 delay 초가 되도록 미리 곱해 둔다.
            return _appearSequence.SetDelay(delay * _appearSequence.timeScale).Play();
        }

        private void OnEnable()
        {
            OnGlobalSpeedChanged += ApplyTimeScale;
            ApplyTimeScale();
        }

        private void OnDisable()
        {
            OnGlobalSpeedChanged -= ApplyTimeScale;
        }

        // 출력 도중 전역 배율(설정)이 바뀌어도 진행 중인 등장 연출에 바로 반영되도록 timeScale 로 처리한다.
        private void ApplyTimeScale()
        {
            // Stop() 으로 Kill 된 시퀀스는 건드리지 않는다.
            if (_appearSequence == null || !_appearSequence.IsActive())
            {
                return;
            }

            _appearSequence.timeScale = _speed * GlobalSpeedMultiplier;
        }

        public void Stop()
        {
            if (_appearSequence != null)
            {
                _appearSequence.Kill();
            }

            if (_letters != null)
            {
                foreach (var letter in _letters)
                {
                    letter.KillRepeatTween();
                }
            }

            _isPlaying = false;
            //_textComponent.text = "";
        }


        public Sequence GenerateAppearSequence(Letter[] letters)
        {
            var sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            
            foreach (var letter in letters)
            {
                sequence.Append(letter.AppearSequence().AppendCallback(() => { letter.RepeatSequence(); }));
            }

            return sequence;
        }

        public Letter[] GenerateLetter(string text)
        {
            List<Letter> result = new List<Letter>();

            // 줄바꿈(\n)을 포함하여 중첩된 태그를 완벽하게 추적하는 패턴
            string tagPattern =
                @"<(?<tag>\w+)>(?<value>(?:[^<>]+|<(?<Open>\w+)[^>]*>|<\/(?<-Open>\w+)>)*(?(Open)(?!)))<\/\1>" + // 1. 쌍을 이루는 태그
                @"|" +
                @"(?<tag>br|hr|img)\b[^>]*\/?>" + // 2. <br> 같은 단독 태그 추가 (예시)
                @"|" +
                @"(?<text>[^<>]+)"; // 3. 일반 텍스트

            MatchCollection matches =
                Regex.Matches(text, tagPattern, RegexOptions.Multiline, TimeSpan.FromSeconds(5.0));

            foreach (Match match in matches)
            {
                // 태그 형태인 경우 (<tag>value</tag>)
                if (match.Groups["tag"].Success)
                {
                    string styleName = match.Groups["tag"].Value;
                    string value = match.Groups["value"].Value;

                    PrintStyle style = PrinterManager.FindDialogStyle(styleName) ?? PrinterManager.defaultStyle;

                    if (style == PrinterManager.defaultStyle)
                    {
                        value = match.Value;
                    }

                    foreach (var c in value)
                    {
                        result.Add(new Letter(c, style, textComponent.color));
                    }
                }
                // 일반 텍스트인 경우
                else if (match.Groups["text"].Success)
                {
                    string value = match.Groups["text"].Value;

                    foreach (var c in value)
                    {
                        result.Add(new Letter(c, PrinterManager.defaultStyle, textComponent.color));
                    }
                }
            }

            return result.ToArray();
        }

        private void LateUpdate()
        {
            if (_letters != null && textComponent.text.Length > 0)
            {
                textComponent.ForceMeshUpdate();

                var mesh = textComponent.mesh;

                var textInfo = textComponent.textInfo;

                Vector3[] vertices = mesh.vertices;

                Color[] colors = mesh.colors;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var characterInfo = textInfo.characterInfo[i];

                    if (!characterInfo.isVisible)

                    {
                        continue;
                    }

                    var addOffsetX = (_letters[i].scale.x - 1) * i;
                    var addOffsetY = (_letters[i].scale.y - 1) * i;
                    Vector3 center = new Vector3(addOffsetX, addOffsetY, 0);

                    float halfHeight, halfWidth;


                    halfHeight = Vector3.Distance(vertices[characterInfo.vertexIndex],
                        vertices[characterInfo.vertexIndex + 1]) / 2;

                    halfWidth = Vector3.Distance(vertices[characterInfo.vertexIndex + 1],
                        vertices[characterInfo.vertexIndex + 2]) / 2;


                    for (int j = 0; j < 4; j++)
                    {
                        var origin = vertices[characterInfo.vertexIndex + j];
                        center += origin;
                    }


                    center /= 4;

                    vertices[characterInfo.vertexIndex] = center + _letters[i].position +
                                                          Quaternion.Euler(_letters[i].rotation) *
                                                          new Vector3(-halfWidth * _letters[i].scale.x,
                                                              -halfHeight * _letters[i].scale.y, 0);

                    vertices[characterInfo.vertexIndex + 1] = center + _letters[i].position +
                                                              Quaternion.Euler(_letters[i].rotation) *
                                                              new Vector3(-halfWidth * _letters[i].scale.x,
                                                                  halfHeight * _letters[i].scale.y, 0);

                    vertices[characterInfo.vertexIndex + 2] = center + _letters[i].position +
                                                              Quaternion.Euler(_letters[i].rotation) *
                                                              new Vector3(halfWidth * _letters[i].scale.x,
                                                                  halfHeight * _letters[i].scale.y, 0);

                    vertices[characterInfo.vertexIndex + 3] = center + _letters[i].position +
                                                              Quaternion.Euler(_letters[i].rotation) *
                                                              new Vector3(halfWidth * _letters[i].scale.x,
                                                                  -halfHeight * _letters[i].scale.y, 0);

                    colors[characterInfo.vertexIndex] = _letters[i].color;

                    colors[characterInfo.vertexIndex + 1] = _letters[i].color;

                    colors[characterInfo.vertexIndex + 2] = _letters[i].color;

                    colors[characterInfo.vertexIndex + 3] = _letters[i].color;

                    int matIndex = PrinterManager.GetStyleIndex(_letters[i].style);
                    textInfo.characterInfo[i].fontAsset = _letters[i].style.font;
                    textInfo.characterInfo[i].materialReferenceIndex = matIndex;
                }

                mesh.colors = colors;
                mesh.vertices = vertices;
                textComponent.canvasRenderer.SetMesh(mesh);
            }
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }
    }
}