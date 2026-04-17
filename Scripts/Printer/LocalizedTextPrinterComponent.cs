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
      
        public void Awake()
        {
            _printer = GetComponent<Printer>();
            _textComponent = GetComponent<TMP_Text>();
        }
        
        public override void OnChange()
        {
            _textComponent.font = localizationManager.GetFontAsset();
            _printer.Stop();
            _printer.Setup(localizationManager.GetLocalizedText(key));
            _printer.Print(Random.Range(0.0f,1.0f));
        }
        
    }
}