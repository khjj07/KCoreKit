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
  
        public string name = "default";

        public AdvancedPrintOption option;

        public PrintStyleAppearGroup appear;
        public PrintStyleRepeatGroup repeat;
        public PrintStyleDisappearGroup disappear;
    }
}