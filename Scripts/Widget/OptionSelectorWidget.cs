using System;
using KCoreKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    public class OptionSelectorWidget : WidgetBase
    {
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text valueText;
        
        private string[] _options;
        private int _currentIndex;

        public event Action<int> OnIndexChanged;

        public void Awake()
        {
            prevButton.onClick.AddListener(OnClickPrevious);
            nextButton.onClick.AddListener(OnClickNext);
        }

        public void Setup(string[] options, int initialIndex = 0)
        {
            _options = options;
            _currentIndex = initialIndex;
            Refresh();
        }

        public void SetIndex(int index)
        {
            _currentIndex = (index - 1 + _options.Length) % _options.Length;;
            Refresh();
            OnIndexChanged?.Invoke(_currentIndex);
        }
        
        public void SetIndexWithoutNotify(int value)
        {
            _currentIndex = value;
            Refresh();
        }

        private void OnClickPrevious()
        {
            _currentIndex = (_currentIndex - 1 + _options.Length) % _options.Length;
            Refresh();
            OnIndexChanged?.Invoke(_currentIndex);
        }

        private void OnClickNext()
        {
            _currentIndex = (_currentIndex + 1) % _options.Length;
            Refresh();
            OnIndexChanged?.Invoke(_currentIndex);
        }

        private void Refresh()
        {
            valueText.text = _options[_currentIndex];
        }
    }
}
