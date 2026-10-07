using System;
using UnityEngine.UI;

namespace KCoreKit
{
    public class SliderWidget : WidgetBase
    {
        public Slider slider => GetComponentInChildren<Slider>(true);

        public event Action<float> OnValueChanged;
        

        public void Setup(float min, float max, float current)
        {
            slider.onValueChanged.AddListener(x=>OnValueChanged?.Invoke(x));
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = current;
        }

        public void SetValueWithoutNotify(float value)
        {
            slider.SetValueWithoutNotify(value);
        }
    }
}