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


        public Tween Print(float delay = 0, TweenCallback callback = null)
        {
            if (_appearSequence == null)
            {
                Debug.LogWarning($"{nameof(Printer)}.{nameof(Print)} called before {nameof(Setup)}.", this);
                return DOTween.Sequence().Play();
            }

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
            return _appearSequence.SetDelay(delay).Play();
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