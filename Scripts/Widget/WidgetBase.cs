using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace KCoreKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WidgetBase), true)]
    public class WidgetBaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var widgetBase = (WidgetBase)target;
            base.OnInspectorGUI();
            if (widgetBase.isShown)
            {
                if (GUILayout.Button("Hide"))
                {
                    widgetBase.canvasGroup = widgetBase.GetComponent<CanvasGroup>();
                    widgetBase.rectTransform = widgetBase.GetComponent<RectTransform>();
                    widgetBase.Hide();
                }
            }
            else
            {
                if (GUILayout.Button("Show"))
                {
                    widgetBase.canvasGroup = widgetBase.GetComponent<CanvasGroup>();
                    widgetBase.rectTransform = widgetBase.GetComponent<RectTransform>();
                    widgetBase.Show();
                }
            }
        }
    }
#endif

    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class WidgetBase : MonoBehaviour
    {
        [HideInInspector] public RectTransform rectTransform;
        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public Canvas canvas;
        protected bool isAwake = false;
        public bool isShown => gameObject.activeSelf;

        public virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();
            isAwake = true;
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void SetPosition(UnityEngine.Camera camera, Vector3 position, Vector3 offset)
        {
            var screenPosition = camera.WorldToScreenPoint(position + offset);
            rectTransform.anchoredPosition = screenPosition;
        }

        public void SetSize(UnityEngine.Camera camera, float  size)
        {
            float scaleFactor = size / camera.orthographicSize;
            rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }
}