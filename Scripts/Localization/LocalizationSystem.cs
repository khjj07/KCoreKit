using System.Collections;
using System.Collections.Generic;

namespace KCoreKit
{
    public enum Language
    {
        EN,
        KR,
        JP,
        CN
    }

    public class LocalizationSystem : GameSubSystemBase
    {
        public Language language;

        private List<LocalizedTextDataTableRow> _textDataTableRows;
        private List<LocalizedFontDataTableRow> _fontDataTableRows;

        public IEnumerator OnInitialize()
        {
            var dataTableSystem = GameSystem.GetSubSystem<DataTableSystem>();
            _textDataTableRows = dataTableSystem.FindAllRows<LocalizedTextDataTableRow>();
            _fontDataTableRows = dataTableSystem.FindAllRows<LocalizedFontDataTableRow>();
            yield return null;
        }

        public string GetLocalizedText(string key)
        {
          return _textDataTableRows.Find(x => x.id == key).GetText(language);
        }
    }
}