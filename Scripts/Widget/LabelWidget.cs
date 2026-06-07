using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class LabelWidget : WidgetBase
    {
        [HideInInspector]
        public TMP_Text label =>GetComponentInChildren<TextMeshProUGUI>(true);

        public void SetText(string s)
        {
            label.text = s;
        }

        public void SetFont(TMP_FontAsset font)
        {
            label.font = font;
        }
    }
}