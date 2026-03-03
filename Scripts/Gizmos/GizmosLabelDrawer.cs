using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    public class GizmosLabelDrawer : MonoBehaviour
    {
        public string text;
        public Vector3 offset;
        public Color textColor;
        public int fontSize;
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize; // 글씨 크기 설정
            style.normal.textColor =textColor; // 글씨 색상 설정
            style.alignment = TextAnchor.MiddleCenter;
            Handles.Label(transform.position + offset, text,style);
        }
#endif
    }
}