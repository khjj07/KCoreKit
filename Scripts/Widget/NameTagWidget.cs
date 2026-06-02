using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class NameTagWidget : WidgetBase
    {
        [HideInInspector]
        public TMP_Text label =>GetComponentInChildren<TextMeshProUGUI>(true);

    }
}