using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    public class ReadOnlyTextAreaAttribute : PropertyAttribute
    {
        public readonly int minLines;
        public readonly int maxLines;

        public ReadOnlyTextAreaAttribute(int minLines = 3, int maxLines = 5)
        {
            this.minLines = minLines;
            this.maxLines = maxLines;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyTextAreaAttribute))]
    public class ReadOnlyTextAreaDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyTextAreaAttribute textArea = (ReadOnlyTextAreaAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "ReadOnlyTextArea는 string에만 사용할 수 있습니다.");
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            property.stringValue = EditorGUI.TextArea(position, property.stringValue, EditorStyles.textArea);
            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReadOnlyTextAreaAttribute textArea = (ReadOnlyTextAreaAttribute)attribute;
            
            return EditorGUIUtility.singleLineHeight * Mathf.Clamp(textArea.maxLines, textArea.minLines, 20);
        }
    }
#endif
}