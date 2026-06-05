using TMPro;
using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextComponent : LocalizedComponentBase
    {
        private TMP_Text _textComponent;
        public string key;
      
        public override void Awake()
        {
            base.Awake();
            _textComponent = GetComponent<TMP_Text>();
        }
        
        public override void OnChange()
        {
            _textComponent.font = LocalizationManager.GetFontAsset(0);
            _textComponent.text = LocalizationManager.GetLocalizedText(key);
        }
    }
}