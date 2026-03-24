using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public class AdvancedPrintOption
    {
        public float interval = 0.1f;
        public bool usePosition;
        public bool useRotation;
        public bool useScale;
        public bool useColor;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PrintStyle))]
    public class PrintStyleInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty appearOptionProp = serializedObject.FindProperty("appearOption");
            EditorGUILayout.PropertyField(appearOptionProp, true);

            if (appearOptionProp.FindPropertyRelative("usePosition").boolValue)
            {
                SerializedProperty appearPosition = serializedObject.FindProperty("appearPosition");
                EditorGUILayout.PropertyField(appearPosition, true);
            }

            if (appearOptionProp.FindPropertyRelative("useRotation").boolValue)
            {
                SerializedProperty appearRotation = serializedObject.FindProperty("appearRotation");
                EditorGUILayout.PropertyField(appearRotation, true);
            }

            if (appearOptionProp.FindPropertyRelative("useScale").boolValue)
            {
                SerializedProperty appearScale = serializedObject.FindProperty("appearScale");
                EditorGUILayout.PropertyField(appearScale, true);
            }

            if (appearOptionProp.FindPropertyRelative("useColor").boolValue)
            {
                SerializedProperty appearColor = serializedObject.FindProperty("appearColor");
                EditorGUILayout.PropertyField(appearColor, true);
            }

            SerializedProperty repeatOptionProp = serializedObject.FindProperty("repeatOption");
            EditorGUILayout.PropertyField(repeatOptionProp, true);
            SerializedProperty repeatOffset = serializedObject.FindProperty("repeatOffset");
            var style = target as PrintStyle;
            style.repeatOffset = EditorGUILayout.FloatField("Repeat Offset", repeatOffset.floatValue);

            if (repeatOptionProp.FindPropertyRelative("usePosition").boolValue)
            {
                SerializedProperty repeatPosition = serializedObject.FindProperty("repeatPosition");
                EditorGUILayout.PropertyField(repeatPosition, true);
            }

            if (repeatOptionProp.FindPropertyRelative("useRotation").boolValue)
            {
                SerializedProperty repeatRotation = serializedObject.FindProperty("repeatRotation");
                EditorGUILayout.PropertyField(repeatRotation, true);
            }

            if (repeatOptionProp.FindPropertyRelative("useScale").boolValue)
            {
                SerializedProperty repeatScale = serializedObject.FindProperty("repeatScale");
                EditorGUILayout.PropertyField(repeatScale, true);
            }

            if (repeatOptionProp.FindPropertyRelative("useColor").boolValue)
            {
                SerializedProperty repeatColor = serializedObject.FindProperty("repeatColor");
                EditorGUILayout.PropertyField(repeatColor, true);
            }

            SerializedProperty disappearOptionProp = serializedObject.FindProperty("disappearOption");
            EditorGUILayout.PropertyField(disappearOptionProp, true);

            if (disappearOptionProp.FindPropertyRelative("usePosition").boolValue)
            {
                SerializedProperty disappearPosition = serializedObject.FindProperty("disappearPosition");
                EditorGUILayout.PropertyField(disappearPosition, true);
            }

            if (disappearOptionProp.FindPropertyRelative("useRotation").boolValue)
            {
                SerializedProperty disappearRotation = serializedObject.FindProperty("disappearRotation");
                EditorGUILayout.PropertyField(disappearRotation, true);
            }

            if (disappearOptionProp.FindPropertyRelative("useScale").boolValue)
            {
                SerializedProperty disappearScale = serializedObject.FindProperty("disappearScale");
                EditorGUILayout.PropertyField(disappearScale, true);
            }

            if (disappearOptionProp.FindPropertyRelative("useColor").boolValue)
            {
                SerializedProperty disappearColor = serializedObject.FindProperty("disappearColor");
                EditorGUILayout.PropertyField(disappearColor, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
    [CreateAssetMenu(menuName = "Printer/Print Style", fileName = "new Print Style")]
    public class PrintStyle : ScriptableObject
    {
        [BigHeader("Appear")] public AdvancedPrintOption appearOption;
        public PrintStylePositionModifier appearPosition;
        public PrintStyleRotationModifier appearRotation;
        public PrintStyleScaleModifier appearScale;
        public PrintStyleColorModifier appearColor;

        [BigHeader("Repeat")] public AdvancedPrintOption repeatOption;

        public float repeatOffset = 0.1f;
        public PrintStylePositionModifier repeatPosition;
        public PrintStyleRotationModifier repeatRotation;
        public PrintStyleScaleModifier repeatScale;
        public PrintStyleColorModifier repeatColor;

        [BigHeader("Disappear")] public AdvancedPrintOption disappearOption;
        public PrintStylePositionModifier disappearPosition;
        public PrintStyleRotationModifier disappearRotation;
        public PrintStyleScaleModifier disappearScale;
        public PrintStyleColorModifier disappearColor;
    }
}