using System;
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public class AdvancedPrintOption
    {
        public float repeatOffset = 0.1f;
        public bool usePosition;
        public bool useScale;
        public bool useRotation;
        public bool useColor;
    }


    [CreateAssetMenu(menuName = "Printer/Print Style", fileName = "new Print Style")]
    public class PrintStyle : ScriptableObject
    {
        public AdvancedPrintOption option;
        [BigHeader("Appear")]
        public PrintStyleAppearGroup appear;
        [BigHeader("Repeat")]
        public PrintStyleRepeatGroup repeat;
        [BigHeader("Disappear")]
        public PrintStyleDisappearGroup disappear;
    }
}