using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Image))]
    public class BoxWidget : WidgetBase
    {
        [HideInInspector]
        public Image image =>GetComponent<Image>();
    }
}