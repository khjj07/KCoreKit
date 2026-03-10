using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
#if UNITY_EDITOR

    [CustomEditor(typeof(DataTableRowBase), true)]
    public class DataTableRowBaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
#endif
    
    public abstract class DataTableRowBase : ScriptableObject
    {
        public string id;
        public bool isEnable;
    }

}