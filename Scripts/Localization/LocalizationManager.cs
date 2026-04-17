using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public enum Language
    {
        EN,
        KR,
        JP,
        CN
    }

    public class LocalizationManager : Singleton<LocalizationManager>
    {
        [SerializeField] private Language defaultLanguage = Language.EN;

        private static Language _language;

        private List<LocalizedTextDataTableRow> _textDataTableRows;
        private List<LocalizedFontDataTableRow> _fontDataTableRows;
        private List<LocalizedSpriteDataTableRow> _spriteDataTableRows;
        private List<LocalizedPrefabDataTableRow> _prefabDataTableRows;

        public static Action onChange;

        public void Start()
        {
            DataTableManager.AddOnLoadAction(() =>
            {
                _textDataTableRows = DataTableManager.FindAllRows<LocalizedTextDataTableRow>();
                _fontDataTableRows = DataTableManager.FindAllRows<LocalizedFontDataTableRow>();
                _spriteDataTableRows = DataTableManager.FindAllRows<LocalizedSpriteDataTableRow>();
                _prefabDataTableRows = DataTableManager.FindAllRows<LocalizedPrefabDataTableRow>();
                SetLanguage(defaultLanguage);
            });
        }

        public static void SetLanguage(Language language)
        {
            _language = language;
            onChange?.Invoke();
        }

        public Language GetLanguage()
        {
            return _language;
        }

        public string GetLocalizedText(string key)
        {
            return _textDataTableRows.Find(x => x.id == key).Get(_language);
        }

        public TMP_FontAsset GetFontAsset()
        {
            return _fontDataTableRows.Find(x => x.id == _language.ToString()).fontAsset;
        }

        public Sprite GetLocalizedSprite(string key)
        {
            return _spriteDataTableRows.Find(x => x.id == key).Get(_language);
        }

        public GameObject GetLocalizedPrefab(string key)
        {
            return _prefabDataTableRows.Find(x => x.id == key).Get(_language);
        }
    }
}