using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)] // 모든 MonoBehaviour를 상속받는 클래스에 적용
    [CanEditMultipleObjects]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var ba = (ButtonAttribute)System.Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));

                if (ba != null)
                {
                    string label = string.IsNullOrEmpty(ba.ButtonLabel) ? method.Name : ba.ButtonLabel;

                    if (GUILayout.Button(label))
                    {
                        method.Invoke(target, null);
                    }
                }
            }
        }
    }

#endif

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string ButtonLabel { get; }

        public ButtonAttribute(string label = null)
        {
            ButtonLabel = label;
        }
    }
}