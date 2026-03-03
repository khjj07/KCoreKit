using TMPro;
using UnityEngine;
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
    }
}