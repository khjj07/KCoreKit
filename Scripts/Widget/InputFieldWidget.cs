using System;
using TMPro;

namespace KCoreKit
{
    public class InputFieldWidget : WidgetBase
    {
        public TMP_InputField component => GetComponent<TMP_InputField>();
        public event Action<string> OnValueChanged;

        public void Awake()
        {
            component.onValueChanged.AddListener(x=>OnValueChanged?.Invoke(x));
        }

        public string GetText()
        {
          return component.text;
        }
    }
}