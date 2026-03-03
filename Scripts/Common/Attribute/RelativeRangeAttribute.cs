using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    // 필드 간의 상대적인 값 제약을 슬라이더로 표시해주는 Attribute
    public class RelativeRangeAttribute : PropertyAttribute
    {
        public string otherFieldName;
        public bool mustBeGreater;
        public float limit;

        public RelativeRangeAttribute(string otherFieldName, bool mustBeGreater, float limit)
        {
            this.otherFieldName = otherFieldName;
            this.mustBeGreater = mustBeGreater;
            this.limit = limit;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RelativeRangeAttribute))]
    public class RelativeRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RelativeRangeAttribute attr = (RelativeRangeAttribute)attribute;
            SerializedProperty otherProp = FindRelativeProperty(property, attr.otherFieldName);

            if (otherProp == null)
            {
                EditorGUI.LabelField(position, label.text, $"필드 '{attr.otherFieldName}' 을(를) 찾을 수 없습니다.");
                return;
            }

            if (property.propertyType == SerializedPropertyType.Float && otherProp.propertyType == SerializedPropertyType.Float)
            {
                float value = property.floatValue;
                float other = otherProp.floatValue;

                float min = attr.mustBeGreater ? other : attr.limit;
                float max = attr.mustBeGreater ? attr.limit : other;

                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.Slider(position, label, value, min, max);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Mathf.Clamp(newValue, min, max);
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer && otherProp.propertyType == SerializedPropertyType.Integer)
            {
                int value = property.intValue;
                int other = otherProp.intValue;

                int min = attr.mustBeGreater ? (int)other : (int)attr.limit;
                int max = attr.mustBeGreater ? (int)attr.limit : (int)other;

                EditorGUI.BeginChangeCheck();
                int newValue = EditorGUI.IntSlider(position, label, value, min, max);
                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = Mathf.Clamp(newValue, min, max);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private SerializedProperty FindRelativeProperty(SerializedProperty property, string relativeName)
        {
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot >= 0)
            {
                string newPath = path.Substring(0, lastDot) + "." + relativeName;
                return property.serializedObject.FindProperty(newPath);
            }

            return property.serializedObject.FindProperty(relativeName);
        }
    }

#endif
}