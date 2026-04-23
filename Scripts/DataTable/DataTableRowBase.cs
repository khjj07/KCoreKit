using System.Collections.Generic;
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
        [SerializeField, HideInInspector]
        private Dictionary<string, string> rawData;
        public string id;
        public bool isEnable;
        
        public void SetRawData(Dictionary<string, string> row)
        {
            rawData = row;
        }

        public Dictionary<string, string> GetRawData()
        {
            return rawData;
        }
    }

}