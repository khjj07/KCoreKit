using TMPro;
using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextPrinterComponent : LocalizedComponentBase
    {
        private Printer _printer;
        private TMP_Text _textComponent;
        public string key;
      
        public override void Awake()
        {
            base.Awake();
            _printer = GetComponent<Printer>();
            _textComponent = GetComponent<TMP_Text>();
        }
        
        public override void OnChange()
        {
            _textComponent.font = LocalizationManager.GetFontAsset(0);
            _printer.Stop();
            _printer.Setup(LocalizationManager.GetLocalizedText(key));
            _printer.Print(Random.Range(0.0f,1.0f));
        }
        
    }
}