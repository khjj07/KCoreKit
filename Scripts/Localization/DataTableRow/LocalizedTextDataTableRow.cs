using System;
using KCoreKit;
using UnityEngine;

public class LocalizedTextDataTableRow : LocalizedDataTableRowBase<string>
{
    [TextArea]
    public string EN;
    [TextArea]
    public string KR;
    [TextArea]
    public string JP;
    [TextArea]
    public string CN;

    public override string Get(Language language)
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