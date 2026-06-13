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
        public List<string> tags;
        public bool isEnable;
        
        public void SetRawData(Dictionary<string, string> row)
        {
            rawData = row;
        }

        public Dictionary<string, string> GetRawData()
        {
            return rawData;
        }
        
        public bool ContainTags(List<string> list)
        {
            foreach (var tag in list)
            {
                if (!tags.Contains(tag))
                {
                    return false;
                }
            }

            return true;
        }
    }

}