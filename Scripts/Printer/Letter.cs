using System;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public class Letter
    {
        public PrintStyle style;
        public char value;

        public Vector3 position;
        public Vector3 scale;
        public Vector3 rotation;
        public Color color;

        public Tween repeatPositionTween;
        public Tween repeatScaleTween;
        public Tween repeatRotationTween;
        public Tween repeatColorTween;
        public Letter(char value, PrintStyle style)
        {
            this.value = value;
            this.style = style;
        }

        public Sequence AppearSequence()
        {
            var sequence = DOTween.Sequence();
            var appearStyle = style.appear;

            position = appearStyle.beginPosition;
            scale = appearStyle.beginScale;
            rotation = appearStyle.beginRotation;
            color = appearStyle.beginColor;
            if (style.option.usePosition)
            {
                sequence.Join(DOTween.To(() => position, x => position = x, appearStyle.endPosition,
                        appearStyle.interval / appearStyle.positionSpeed)
                    .SetEase(appearStyle.positionEase));
            }

            if (style.option.useScale)
            {
                sequence.Join(DOTween
                    .To(() => scale, x => scale = x, appearStyle.endScale, appearStyle.interval / appearStyle.scaleSpeed)
                    .SetEase(appearStyle.scaleEase));
            }

            if (style.option.useRotation)
            {

                sequence.Join(DOTween
                    .To(() => rotation, x => rotation = x, appearStyle.endRotation,
                        appearStyle.interval / appearStyle.rotationSpeed)
                    .SetEase(appearStyle.rotationEase));
            }

            if (style.option.useColor)
            {
                sequence.Join(DOTween.To(() => color, x => color = x, appearStyle.endColor,
                    appearStyle.interval / appearStyle.colorSpeed)
                    .SetEase(appearStyle.colorEase));
            }

            return sequence;
        }

        public Sequence RepeatSequence()
        {
            var sequence = DOTween.Sequence();
            var repeatStyle = style.repeat;

            sequence.AppendInterval(style.option.repeatOffset).AppendCallback(() =>
            {
                if (style.option.usePosition)
                {
                    repeatPositionTween = DOTween
                        .To(() => position, x => position = x, repeatStyle.endPosition,
                            repeatStyle.interval / repeatStyle.positionSpeed).SetEase(repeatStyle.positionEase)
                        .SetLoops(-1, LoopType.Yoyo);
                    repeatPositionTween.Play();
                }

                if (style.option.useScale)
                {
                    repeatScaleTween = DOTween
                        .To(() => scale, x => scale = x, repeatStyle.endScale,
                            repeatStyle.interval / repeatStyle.scaleSpeed).SetEase(repeatStyle.scaleEase)
                        .SetLoops(-1, LoopType.Yoyo);
                    repeatScaleTween.Play();
                }

                if (style.option.useRotation)
                {
                    repeatRotationTween = DOTween
                        .To(() => rotation, x => rotation = x, repeatStyle.endRotation,
                            repeatStyle.interval / repeatStyle.rotationSpeed).SetEase(repeatStyle.rotationEase)
                        .SetLoops(-1, LoopType.Yoyo);
                    repeatRotationTween.Play();
                }

                if (style.option.useColor)
                {
                    repeatColorTween = DOTween
                        .To(() => color, x => color = x, repeatStyle.endColor,
                            repeatStyle.interval / repeatStyle.colorSpeed).SetEase(repeatStyle.colorEase)
                        .SetLoops(-1, LoopType.Yoyo);
                    repeatColorTween.Play();
                }
            });

            return sequence;
        }

        public Sequence DisappearSequence()
        {
            var sequence = DOTween.Sequence();
            var disappearStyle = style.disappear;

            sequence.Append(DOTween.To(() => position, x => position = x, disappearStyle.endPosition, disappearStyle.interval / disappearStyle.positionSpeed)
                    .SetEase(disappearStyle.positionEase))
                .Join(DOTween.To(() => scale, x => scale = x, disappearStyle.endScale, disappearStyle.interval / disappearStyle.scaleSpeed)
                    .SetEase(disappearStyle.scaleEase))
                .Join(DOTween.To(() => rotation, x => rotation = x, disappearStyle.endRotation, disappearStyle.interval / disappearStyle.rotationSpeed)
                    .SetEase(disappearStyle.rotationEase))
                .Join(DOTween.To(() => color, x => color = x, disappearStyle.endColor, disappearStyle.interval / disappearStyle.colorSpeed)
                    .SetEase(disappearStyle.colorEase));
            return sequence;
        }

        public void KillRepeatTween()
        {
            if (repeatPositionTween != null)
            {
                repeatPositionTween.Kill();
                repeatScaleTween.Kill();
                repeatRotationTween.Kill();
                repeatColorTween.Kill();
            }
        }
    }
}