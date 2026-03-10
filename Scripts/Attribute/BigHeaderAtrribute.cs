using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KCoreKit
{
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BigHeaderAttribute : PropertyAttribute
    {
        public string _Text
        {
            get { return mText; }
        }

        private string mText = String.Empty;

        public BigHeaderAttribute(string text)
        {
            mText = text;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(BigHeaderAttribute))]
    public class BigHeaderAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            BigHeaderAttribute attributeHandle = (BigHeaderAttribute)attribute;

            // 1. 텍스트 기반으로 색상 생성
            Color32 headerColor = GetColorFromText(attributeHandle._Text);

            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;

           position = EditorGUI.IndentedRect(position);

            GUIStyle headerTextStyle = new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            
            headerTextStyle.normal.textColor = headerColor;
            GUI.Label(position, attributeHandle._Text, headerTextStyle);
            EditorGUI.DrawRect(new Rect(position.xMin, position.yMin, position.width, 1), headerColor);
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
        
        private Color32 GetColorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new Color32(125, 100, 250, 255); // 기본 색상 반환
            }
            
            int hash = text.GetHashCode();

            byte r = (byte)((hash & 0xFF0000) >> 16);
            byte g = (byte)((hash & 0x00FF00) >> 8);
            byte b = (byte)(hash & 0x0000FF);
            
            r = (byte)(r % 100 + 100);
            g = (byte)(g % 100 + 100);
            b = (byte)(b % 100 + 100);

            return new Color32(r, g, b, 255);
        }
    }
#endif
}
