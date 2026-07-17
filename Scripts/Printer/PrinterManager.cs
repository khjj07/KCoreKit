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
        public static PrintStyle defaultStyle => FindDialogStyle("default");

        private static PrintStyle[] styles;
        public Material[] fontSharedMaterials;
        public void Awake()
        {
            styles = Resources.LoadAll<PrintStyle>("");
            fontSharedMaterials = new Material[styles.Length];
            for (int i = 0; i < styles.Length; i++)
            {
                fontSharedMaterials[i] = styles[i].font.material;
            }
        }

        public static Material[] GetFontSharedMaterials()
        {
            return GetInstance().fontSharedMaterials;
        }

        public static int GetStyleIndex(PrintStyle style)
        {
            return Array.IndexOf(styles,style);
        }


        public static PrintStyle FindDialogStyle(string name)
        {
            return styles.ToList().Find((x) => x.name == name);
        }
    }
}