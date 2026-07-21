using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class TextWidget : WidgetBase
    {
        [HideInInspector]
        public TMP_Text textComponent =>GetComponentInChildren<TextMeshProUGUI>(true);

        public void SetText(string s)
        {
            textComponent.text = s;
        }

        public void SetFont(TMP_FontAsset font)
        {
            textComponent.font = font;
        }

        public void SetColor(Color white)
        {
            textComponent.color = white;
        }
    }
}