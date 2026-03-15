using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonWidget : BoxWidget
    {
        [HideInInspector]
        public Button button => GetComponent<Button>();
        
        [HideInInspector]
        public TMP_Text label => GetComponentInChildren<TMP_Text>(true);

        public void AddOnClickAction(UnityAction action)
        {
            button.onClick.AddListener(action);
        }
    }
}