using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonWidget : WidgetBase
    {
        [HideInInspector]
        public Button button => GetComponent<Button>();
        
        [HideInInspector]
        public Image image => GetComponentInChildren<Image>(true);
        
        [HideInInspector]
        public TMP_Text textComponent => GetComponentInChildren<TMP_Text>(true);

        public Action onClickAction;

        public void Awake()
        {
            button.onClick.AddListener(OnClick);
        }

        protected virtual void OnClick()
        {
            onClickAction?.Invoke();
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }
    }
}