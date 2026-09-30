using System;
using UnityEngine.UI;

namespace KCoreKit
{
    public class SliderWidget : WidgetBase
    {
        public Slider slider => GetComponent<Slider>();

        public event Action<float> OnValueChanged;

        public void Awake()
        {
            slider.onValueChanged.AddListener(x=>OnValueChanged?.Invoke(x));
        }

        public void Setup(float min, float max, float current)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = current;
        }
    }
}