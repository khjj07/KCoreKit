using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Image))]
    public class ImageWidget : WidgetBase
    {
        [HideInInspector]
        public Image image =>GetComponent<Image>();
    }
}