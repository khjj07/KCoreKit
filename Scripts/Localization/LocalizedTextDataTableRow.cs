using System;
using KCoreKit;
using UnityEngine;

public class LocalizedTextDataTableRow : DataTableRowBase
{
    public string EN;
    public string KR;
    public string JP;
    public string CN;

    public string GetText(Language language)
    {
        return language switch
        {
            Language.EN => EN,
            Language.KR => KR,
            Language.JP => JP,
            Language.CN => CN,
            _ => EN
        };
    }
}