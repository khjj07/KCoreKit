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

        public void AddOnClickAction(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void RemoveOnClickAction(Action action)
        {
            button.onClick.RemoveListener(action.Invoke);
        }

        public void ClearOnClickAction()
        {
            button.onClick.RemoveAllListeners();
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }
    }
}