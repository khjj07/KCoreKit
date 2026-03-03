using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class LabelWidget : WidgetBase
    {
        [HideInInspector]
        public TMP_Text label =>GetComponentInChildren<TextMeshProUGUI>(true);
    }
}