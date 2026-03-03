using TMPro;
using UnityEngine;

namespace KCoreKit
{
    public class NameTagWidget : FollowTransformWidget
    {
        [HideInInspector]
        public TMP_Text label =>GetComponentInChildren<TextMeshProUGUI>(true);

    }
}