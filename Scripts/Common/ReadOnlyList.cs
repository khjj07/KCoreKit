using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Linq 사용을 위해 추가

namespace KCoreKit
{

    [Serializable]
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        [SerializeField]
        private List<T> _list = new List<T>();
        
        public T this[int index] => _list[index];
        public int Count => _list.Count;
        
        public void Add(T item) => _list.Add(item);
        public bool Remove(T item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Clear() => _list.Clear();
        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item) => _list.Insert(index, item);
        
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // 생성자
        public ReadOnlyList() { }
        public ReadOnlyList(IEnumerable<T> collection) { _list.AddRange(collection); }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyList<>), true)] 
    public class ReadOnlyListDrawer : PropertyDrawer
    {
        private const float Padding = 2f;
        private const float PaddingY = 5f;
        private const float PaddingX = 15f;

        // PropertyHeight 계산 (이전과 동일)
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty listProperty = property.FindPropertyRelative("_list");
            if (listProperty == null || !listProperty.isArray) { return EditorGUIUtility.singleLineHeight; }
            
            float totalHeight = EditorGUIUtility.singleLineHeight + PaddingY * 2; 
            
            if (property.isExpanded)
            {
                totalHeight += Padding;
                
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    totalHeight += EditorGUI.GetPropertyHeight(elementProperty, true) + Padding; 
                }
                
                if (listProperty.arraySize == 0)
                {
                    totalHeight += EditorGUIUtility.singleLineHeight + Padding;
                }
            }
            return totalHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(20);
            SerializedProperty listProperty = property.FindPropertyRelative("_list");
            if (listProperty == null || !listProperty.isArray)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Error: List field not found."));
                return;
            }
            
 
            Rect contentRect = new Rect(position.x + PaddingX, position.y + 5, position.width - PaddingX * 2, position.height - 10);
            Rect currentRect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, label, true, EditorStyles.foldout); 

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
           
            if (property.isExpanded)
            {
                currentRect.y += EditorGUIUtility.singleLineHeight + Padding;
                
                if (listProperty.arraySize == 0)
                {
                    EditorGUI.LabelField(currentRect, new GUIContent("List is Empty"), EditorStyles.miniLabel);
                    return;
                }
                
                bool wasEnabled = GUI.enabled;
                GUI.enabled = false;

                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    
                    float elementHeight = EditorGUI.GetPropertyHeight(elementProperty, true); 
                   
                    Rect elementRect = new Rect(currentRect.x, currentRect.y, currentRect.width, elementHeight);

                    EditorGUI.PropertyField(elementRect, elementProperty, new GUIContent($"Element {i}"), true);
                    
                    currentRect.y += elementHeight + Padding;
                }

                GUI.enabled = wasEnabled;
            }
        }
    }
#endif
}