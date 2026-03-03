using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace KCoreKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(GlobalPrintSetting))]
    public class GlobalPrinterSettingEditor : Editor
    {
        public void OnValidate()
        {
            var setting = target as GlobalPrintSetting;

            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var setting = target as GlobalPrintSetting;
            if (GUILayout.Button("Load All Styles"))
            {
                setting.styles = setting.styleFolder.LoadAllObjectsInFolder<PrintStyle>().ToArray();
            }
        }
    }
#endif

    [CreateAssetMenu(fileName = "new GlobalPrinterSetting", menuName = "Printer/Global Printer Setting")]
    public class GlobalPrintSetting : SingletonAsset<GlobalPrintSetting>
    {

        public PrintStyle defaultStyle;

        public PrintStyle[] styles;

#if UNITY_EDITOR
        [Space(100)]
        public DefaultAsset styleFolder;
#endif

        public PrintStyle FindDialogStyle(string name)
        {
            return styles.ToList().Find((x) => x.name == name);
        }
    }
}