#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace KCoreKit
{
    // System.Type 사용을 위해 추가

    public class FilterObjectPickerAttribute : PropertyAttribute
    {
        public string filter;
        public bool allowSceneObjects;

        public FilterObjectPickerAttribute(string filter, bool allowSceneObjects = false)
        {
            this.filter = filter;
            this.allowSceneObjects = allowSceneObjects;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FilterObjectPickerAttribute))]
    public class FilterObjectPickerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
 
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            
            bool eventWasConsumed = false; 
            
            if (e.type == EventType.MouseUp && position.Contains(e.mousePosition))
            {
                e.Use(); 
                eventWasConsumed = true; 
        
                System.Type type = fieldInfo.FieldType;
                
                if (type.IsArray)
                {
                    type = type.GetElementType();
                }
                else if (type.IsGenericType && type.GenericTypeArguments.Length > 0)
                {
                    type = type.GenericTypeArguments[0];
                }
                
                if (type == typeof(GameObject))
                {
                    ShowObjectPicker<GameObject>(property.objectReferenceValue, controlID);
                }
                else if (type == typeof(Mesh))
                {
                    ShowObjectPicker<Mesh>(property.objectReferenceValue, controlID);
                }
                else if (type == typeof(Material))
                {
                    ShowObjectPicker<Material>(property.objectReferenceValue, controlID);
                }
                else if (type == typeof(Texture2D))
                {
                    ShowObjectPicker<Texture2D>(property.objectReferenceValue, controlID);
                }
                else if (type == typeof(Sprite))
                {
                    ShowObjectPicker<Sprite>(property.objectReferenceValue, controlID);
                }
                else if (type.BaseType == typeof(ScriptableObject))
                {
                    ShowObjectPicker<ScriptableObject>(property.objectReferenceValue, controlID);
                }
                else
                {
                    ShowObjectPicker<Object>(property.objectReferenceValue, controlID);
                }
            }
            else if (e.type == EventType.ExecuteCommand && e.commandName == "ObjectSelectorUpdated" && controlID == EditorGUIUtility.GetObjectPickerControlID())
            {
                e.Use();
                eventWasConsumed = true; 
                property.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
            }
            
            var att = (FilterObjectPickerAttribute)attribute;
            
            Object newObjectValue = EditorGUI.ObjectField(
                position, 
                label, 
                property.objectReferenceValue, 
                fieldInfo.FieldType, 
                att.allowSceneObjects
            );
            
            if (newObjectValue != property.objectReferenceValue)
            {
                property.objectReferenceValue = newObjectValue;
            }
            
            EditorGUI.EndProperty();
        }

        public void ShowObjectPicker<T>(Object obj, int controlID) where T : Object
        {
            var att = (FilterObjectPickerAttribute)attribute;
            EditorGUIUtility.ShowObjectPicker<T>(obj, att.allowSceneObjects, att.filter, controlID);
        }
    }
#endif
}