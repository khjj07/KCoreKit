using System.Collections.Generic;
using KCoreKit;
using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class LocalizedFontDataTableRow : DataTableRowBase
    {
        public int index;
        public string language;
        public TMP_FontAsset fontAsset;
    }
}