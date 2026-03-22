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
        private Sequence _sequence;
        private TextMeshProUGUI _textComponent;

        public void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }
        public void PreLoad(string text)
        {
            _letters = GenerateLetter(text);
            _textComponent.text = GenerateText();
            _sequence = GenerateSequence(_letters);
        }

        private string GenerateText()
        {
            var builder = new StringBuilder();
            foreach (var letter in _letters)
            {
                builder.Append(letter.value);
            }
            return builder.ToString();
        }

        public void Print(TweenCallback callback = null)
        {
            if (_sequence.IsPlaying())
            {
                return;
            }
            _sequence.Play().OnComplete(() =>
            {
                if (callback != null)
                {
                    callback();
                }
            });
        }

        public void Stop()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
            foreach (var letter in _letters)
            {
                letter.KillRepeatTween();
            }
        }


        public Sequence GenerateSequence(Letter[] letters)
        {
            var sequence = DOTween.Sequence();
            foreach (var letter in letters)
            {
                sequence.Append(letter.AppearSequence());
            }
            foreach (var letter in letters)
            {
                sequence.Append(letter.RepeatSequence());
            }
            return sequence;
        }
        public Letter[] GenerateLetter(string text)
        {
            var setting = GlobalPrintSetting.GetInstance();
            List<Letter> result = new List<Letter>();
            string pattern = @"<(?<tag>\w+)>(?<value>.*?)<\/\w+>|([^<>]+)";
            string pattern1 = @"<(?<tag>\w+)>(?<value>.*?)<\/\w+>";

            MatchCollection matches = Regex.Matches(text, pattern);
            foreach (Match match in matches)
            {
                var realMatch = Regex.Match(match.Value, pattern1);
                if (realMatch.Success)
                {
                    string sytleName = match.Groups["tag"].Value;
                    string value = match.Groups["value"].Value;

                    if (sytleName != "")
                    {
                        PrintStyle style = setting.FindDialogStyle(sytleName);
                        foreach (var c in value)
                        {
                            result.Add(new Letter(c, style));
                        }
                    }
                    else
                    {
                        foreach (var c in value)
                        {
                            result.Add(new Letter(c, setting.defaultStyle));
                        }
                    }
                }
                else
                {
                    foreach (var c in match.Value)
                    {
                        result.Add(new Letter(c, setting.defaultStyle));
                    }
                }
            }
            return result.ToArray();
        }

        private void LateUpdate()
        {
            if (_textComponent.text.Length > 0)
            {
                _textComponent.ForceMeshUpdate();
                var mesh = _textComponent.mesh;
                var textInfo = _textComponent.textInfo;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var characterInfo = textInfo.characterInfo[i];
                    if (!characterInfo.isVisible)
                    {
                        continue;
                    }

                    Vector3 center = Vector3.zero;
                    float halfHeight, halfWidth;

                    halfHeight = Vector3.Distance(vertices[characterInfo.vertexIndex], vertices[characterInfo.vertexIndex + 1]) / 2;
                    halfWidth = Vector3.Distance(vertices[characterInfo.vertexIndex + 1], vertices[characterInfo.vertexIndex + 2]) / 2;

                    for (int j = 0; j < 4; j++)
                    {
                        var origin = vertices[characterInfo.vertexIndex + j];

                        center += origin;
                    }
                    center /= 4;

                    vertices[characterInfo.vertexIndex] = center + _letters[i].position + Quaternion.Euler(_letters[i].rotation) * new Vector3(-halfWidth * _letters[i].scale.x, -halfHeight * _letters[i].scale.y, 0);
                    vertices[characterInfo.vertexIndex + 1] = center + _letters[i].position + Quaternion.Euler(_letters[i].rotation) * new Vector3(-halfWidth * _letters[i].scale.x, halfHeight * _letters[i].scale.y, 0);
                    vertices[characterInfo.vertexIndex + 2] = center + _letters[i].position + Quaternion.Euler(_letters[i].rotation) * new Vector3(halfWidth * _letters[i].scale.x, halfHeight * _letters[i].scale.y, 0);
                    vertices[characterInfo.vertexIndex + 3] = center + _letters[i].position + Quaternion.Euler(_letters[i].rotation) * new Vector3(halfWidth * _letters[i].scale.x, -halfHeight * _letters[i].scale.y, 0);
                    colors[characterInfo.vertexIndex] = _letters[i].color;
                    colors[characterInfo.vertexIndex + 1] = _letters[i].color;
                    colors[characterInfo.vertexIndex + 2] = _letters[i].color;
                    colors[characterInfo.vertexIndex + 3] = _letters[i].color;
                }

                mesh.colors = colors;
                mesh.vertices = vertices;
                _textComponent.canvasRenderer.SetMesh(mesh);
            }
        }
    }


}