using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    public enum TextMode
    {
        Int,
        Float1,
        Float2
    }
    
    public class GaugeWidget : WidgetBase
    {
        public float maxValue;
        public float currentValue;
        public float changeDuration = 0.1f;
        public Image image;
        public TMP_Text textComponent;
        public TextMode textMode;

        public void OnEnable()
        {
            image.fillAmount = 1;
        }

        public void Setup(float maxValue, float currentValue = 0)
        {
            this.maxValue = maxValue;
            this.currentValue = currentValue;
            image.fillAmount = currentValue / maxValue;
            if (textComponent)
            {
                switch (textMode)
                {
                    case TextMode.Int:
                        textComponent.text = $"{currentValue:N0}/{maxValue:N0}";
                        break;
                    case TextMode.Float1:
                        textComponent.text = $"{currentValue:F1}/{maxValue:F1}";
                        break;
                    case TextMode.Float2:
                        textComponent.text = $"{currentValue:F2}/{maxValue:F2}";
                        break;
                }
                
            }
        }

        public void OnChange(float value)
        {
            currentValue = value;
            if (textComponent)
            {
                switch (textMode)
                {
                    case TextMode.Int:
                        textComponent.text = $"{currentValue:N0}/{maxValue:N0}";
                        break;
                    case TextMode.Float1:
                        textComponent.text = $"{currentValue:F1}/{maxValue:F1}";
                        break;
                    case TextMode.Float2:
                        textComponent.text = $"{currentValue:F2}/{maxValue:F2}";
                        break;
                }
            }
            DOTween.To(() => image.fillAmount, x => image.fillAmount = x, currentValue / maxValue, changeDuration);
        }
    }
}