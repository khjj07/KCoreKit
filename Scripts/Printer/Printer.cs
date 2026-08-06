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
        private TMP_Text textComponent => GetComponent<TMP_Text>();
        private bool _isPlaying;

     
        public void Setup(string text, TMP_FontAsset font = null)
        {
            if (font)
            {
                textComponent.font = font;
            }
            
            _letters = GenerateLetter(text);
            _appearSequence = GenerateAppearSequence(_letters);
        }


        public Tween Print(float delay = 0, TweenCallback callback = null)
        {
            if (_isPlaying)
            {
                return null;
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

            foreach (var letter in _letters)
            {
                letter.KillRepeatTween();
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