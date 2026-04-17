using TMPro;
using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextComponent : LocalizedComponentBase
    {
        private TMP_Text _textComponent;
        public string key;
      
        public void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
        }
        
        public override void OnChange()
        {
            _textComponent.font = localizationManager.GetFontAsset();
            _textComponent.text = localizationManager.GetLocalizedText(key);
        }
        
    }
}