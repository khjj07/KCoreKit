using System;
using System.Collections.Generic;
using TMPro;

namespace KCoreKit
{
    public struct DropdownOptionData
    {
        public int index;
        public string text;
    }
    
    public class DropdownWidget : WidgetBase
    {
        private List<DropdownOptionData> _elements;
        public event Action<DropdownOptionData> OnValueChanged;
        public TMP_Dropdown component => GetComponent<TMP_Dropdown>();

        public void Awake()
        {
            component.onValueChanged.AddListener(x=>OnValueChanged?.Invoke(GetValue(x)));
        }

        private DropdownOptionData GetValue(int arg0)
        {
           return _elements[arg0];
        }

        public DropdownOptionData GetCurrentValue()
        {
            return GetValue(component.value);
        }

        public void Setup(List<string> elements)
        {
            _elements=new List<DropdownOptionData>();
            for (int i = 0; i < elements.Count; i++)
            {
                _elements.Add(new DropdownOptionData()
                {
                    index = i,
                    text = elements[i]
                });
            }
            component.options = new List<TMP_Dropdown.OptionData>();
            foreach (var element in _elements)
            {
                component.options.Add(new TMP_Dropdown.OptionData(element.text));
            }
            
        }
    }
}