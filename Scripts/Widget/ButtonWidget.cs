using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonWidget : ImageWidget
    {
        [HideInInspector]
        public Button button => GetComponent<Button>();
        
        [HideInInspector]
        public TMP_Text label => GetComponentInChildren<TMP_Text>(true);

        public void AddOnClickAction(UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public void RemoveOnClickAction(UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }

        public void ClearOnClickAction()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}