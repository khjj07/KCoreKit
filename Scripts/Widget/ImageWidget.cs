using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(Image))]
    public class ImageWidget : WidgetBase
    {
        [HideInInspector]
        public Image image =>GetComponent<Image>();

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetMaterial(Material material)
        {
            image.material = material;
        }
    }
}