using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Linq 사용을 위해 추가

namespace KCoreKit
{
    /// <summary>
    /// List<T>를 감싸서 인스펙터에서만 읽기 전용으로 표시하며,
    /// 런타임 코드에서는 IReadOnlyList<T> 및 자체 메서드를 통해 읽기/쓰기가 가능합니다.
    /// </summary>
    [Serializable]
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        // Unity가 인스펙터에 직렬화할 실제 리스트 필드
        [SerializeField]
        private List<T> _list = new List<T>();

        // 런타임 읽기 전용 속성 (IReadOnlyList<T> 구현)
        public T this[int index] => _list[index];
        public int Count => _list.Count;

        // 💡 [추가] 코드를 통한 수정 메서드
        // 이 메서드들은 내부 리스트의 기능을 직접 호출합니다.
        public void Add(T item) => _list.Add(item);
        public bool Remove(T item) => _list.Remove(item);
        public void RemoveAt(int index) => _list.RemoveAt(index);
        public void Clear() => _list.Clear();
        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item) => _list.Insert(index, item);

        // IReadOnlyList<T> 구현을 위한 열거자
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // 생성자
        public ReadOnlyList() { }
        public ReadOnlyList(IEnumerable<T> collection) { _list.AddRange(collection); }
    }
#if UNITY_EDITOR

    // ReadOnlyListDrawer는 ReadOnlyList<T> 내부의 _list 필드를 찾아
    // 인스펙터에서 강제로 읽기 전용 라벨로만 그리는 역할을 수행합니다.
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
                totalHeight += Padding; // 리스트 헤더와 첫 번째 요소 사이의 패딩
                
                // 각 요소의 높이를 정확히 계산합니다.
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    totalHeight += EditorGUI.GetPropertyHeight(elementProperty, true) + Padding; 
                }
                
                // 리스트가 비어있을 경우 "List is Empty" 라벨을 위한 공간
                if (listProperty.arraySize == 0)
                {
                    totalHeight += EditorGUIUtility.singleLineHeight + Padding;
                }
            }
            return totalHeight;
        }

        // GUI 드로잉
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(20);
            // 1. 실제 리스트 필드 (_list)를 가져옵니다.
            SerializedProperty listProperty = property.FindPropertyRelative("_list");
            if (listProperty == null || !listProperty.isArray)
            {
                EditorGUI.LabelField(position, label, new GUIContent("Error: List field not found."));
                return;
            }

            // A. Box 그리기 및 B. 제목 Toggle/Label 그리기
 
            Rect contentRect = new Rect(position.x + PaddingX, position.y + 5, position.width - PaddingX * 2, position.height - 10);
            Rect currentRect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, label, true, EditorStyles.foldout); 

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
            // 💡 [수정된 부분] Foldout과 Label을 하나로 그립니다. (박스 밖으로 나가는 문제 해결)
           
            // C. 요소 그리기 (펼쳐졌을 때만)
            if (property.isExpanded)
            {
                currentRect.y += EditorGUIUtility.singleLineHeight + Padding;
                
                // 리스트가 비어있는 경우 메시지를 표시합니다.
                if (listProperty.arraySize == 0)
                {
                    EditorGUI.LabelField(currentRect, new GUIContent("List is Empty"), EditorStyles.miniLabel);
                    return;
                }
                
                // E. 요소 값들을 읽기 전용 PropertyField로 그리기 
                bool wasEnabled = GUI.enabled;
                GUI.enabled = false; // 모든 편집을 비활성화

                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    
                    float elementHeight = EditorGUI.GetPropertyHeight(elementProperty, true); 
                    // currentRect.x는 이미 BoxPadding만큼 안쪽으로 들어와 있으므로, 
                    // indentLevel을 조정할 필요 없이 그립니다.
                    Rect elementRect = new Rect(currentRect.x, currentRect.y, currentRect.width, elementHeight);

                    EditorGUI.PropertyField(elementRect, elementProperty, new GUIContent($"Element {i}"), true);
                    
                    currentRect.y += elementHeight + Padding;
                }

                GUI.enabled = wasEnabled; // GUI 상태를 원래대로 복원
            }
        }
    }
#endif
}