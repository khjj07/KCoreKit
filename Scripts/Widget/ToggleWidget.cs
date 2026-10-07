using System;
using UnityEngine.UI;

namespace KCoreKit
{
    public class ToggleWidget : WidgetBase
    {
        public event Action<bool> OnValueChanged;

        public Toggle component => GetComponent<Toggle>();

        public void Awake()
        {
            component.onValueChanged.AddListener(x=>OnValueChanged?.Invoke(x));
        }

        public void SetIsOnWithoutNotify(bool value)
        {
            component.SetIsOnWithoutNotify(value);
        }
    }
    
}