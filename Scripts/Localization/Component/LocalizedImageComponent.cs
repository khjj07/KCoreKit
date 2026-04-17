using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KCoreKit
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LocalizedImageComponent : LocalizedComponentBase
    {
        private Image _image;
        public string key;
      
        public void Awake()
        {
            _image = GetComponent<Image>();
        }

    

        public override void OnChange()
        {
            _image.sprite = localizationManager.GetLocalizedSprite(key);
        }
        
    }
}