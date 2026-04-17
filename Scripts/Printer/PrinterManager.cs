using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KCoreKit
{
    
    public class PrinterManager : Singleton<PrinterManager>
    {
        [HideInInspector]
        public PrintStyle defaultStyle => FindDialogStyle("default");

        private PrintStyle[] styles;

        public void Awake()
        {
            styles = Resources.LoadAll<PrintStyle>("");
        }


        public PrintStyle FindDialogStyle(string name)
        {
            return styles.ToList().Find((x) => x.name == name);
        }
    }
}