using TMPro;
using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextContainer : MonoBehaviour
    {
        private TMP_Text _textComponent;
        public string key;

        public void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
        }

        public void Setup()
        {
            var gameSystem = GameSystem.GetInstance();
            _textComponent.text = gameSystem.GetSubSystem<LocalizationSystem>().GetLocalizedText(key);
        }
    }
}