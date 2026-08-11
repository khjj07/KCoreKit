using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    public class GaugeWidget : WidgetBase
    {
        public float maxValue;
        public float currentValue;
        public float changeDuration = 0.1f;
        public Image image;
        public TMP_Text textComponent;

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
                textComponent.text = $"{currentValue:N0}/{maxValue:N0}";
            }
        }

        public void OnChange(float value)
        {
            currentValue = value;
            if (textComponent)
            {
                textComponent.text = $"{currentValue:N0}/{maxValue:N0}";
            }
            DOTween.To(() => image.fillAmount, x => image.fillAmount = x, currentValue / maxValue, changeDuration);
        }
    }
}