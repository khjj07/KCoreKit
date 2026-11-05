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

        private static List<LocalizedTextDataTableRow> _textDataTableRows;
        private static List<LocalizedFontDataTableRow> _fontDataTableRows;
        private static List<LocalizedSpriteDataTableRow> _spriteDataTableRows;
        private static  List<LocalizedPrefabDataTableRow> _prefabDataTableRows;
        private static  bool _isLoaded;

        public static Action onChange;

        public void Awake()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                DataTableManager.AddOnLoadAction(() =>
                {
                    _textDataTableRows = DataTableManager.FindAllRows<LocalizedTextDataTableRow>();
                    _fontDataTableRows = DataTableManager.FindAllRows<LocalizedFontDataTableRow>();
                    _spriteDataTableRows = DataTableManager.FindAllRows<LocalizedSpriteDataTableRow>();
                    _prefabDataTableRows = DataTableManager.FindAllRows<LocalizedPrefabDataTableRow>();
                    SetLanguage(defaultLanguage);
                });
            }
            
        }

        public static void SetLanguage(Language language)
        {
            _language = language;
            onChange?.Invoke();
        }

        public static Language GetLanguage()
        {
            return _language;
        }

        public static string GetLocalizedText(string key)
        {
            var row = _textDataTableRows.Find(x => x.id == key);
            if (row == null)
            {
                // 키가 테이블에 없으면 예외 대신 키를 그대로 보여줘서 화면에서 바로 찾을 수 있게 한다.
                Debug.LogWarning($"[{nameof(LocalizationManager)}] Missing localized text key: '{key}'");
                return key;
            }

            return row.Get(_language);
        }

        public static TMP_FontAsset GetFontAsset(int index)
        {
            return _fontDataTableRows.Find(x => x.language == _language.ToString() && x.index == index).fontAsset;
        }

        public static Sprite GetLocalizedSprite(string key)
        {
            return _spriteDataTableRows.Find(x => x.id == key).Get(_language);
        }

        public static GameObject GetLocalizedPrefab(string key)
        {
            return _prefabDataTableRows.Find(x => x.id == key).Get(_language);
        }

        public static List<string> GetLanguageNames()
        {
            var languageName = _textDataTableRows.Find(x => x.id == "languageName");
            return new List<string>
            {
                languageName.KR,
                languageName.EN,
                languageName.JP,
                languageName.CN
            };
        }
    }
}