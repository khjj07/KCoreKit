using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
                    widgetBase.Hide();
                }
            }
            else
            {
                if (GUILayout.Button("Show"))
                {
                    widgetBase.Show();
                }
            }
        }
    }
#endif

    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class WidgetBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [HideInInspector] public RectTransform rectTransform => GetComponent<RectTransform>();
        [HideInInspector] public CanvasGroup canvasGroup => GetComponent<CanvasGroup>();
        [HideInInspector] public Canvas canvas => GetComponentInParent<Canvas>();
        protected bool isAwake = false;
        public bool isShown => gameObject.activeSelf;

        public Action<PointerEventData> onPointerClickCallback;
        public Action<PointerEventData> onPointerEnterCallback;
        public Action<PointerEventData> onPointerExitCallback;
        public Action<PointerEventData> onPointerDownCallback;
        public Action<PointerEventData> onPointerUpCallback;
        public Action<PointerEventData> onPointerMoveCallback;

        public virtual void Awake()
        {
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
        
        public void SetParent(Transform parent,bool worldPositionStays = false)
        {
            transform.SetParent(parent,worldPositionStays);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onPointerClickCallback?.Invoke(eventData);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterCallback?.Invoke(eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitCallback?.Invoke(eventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            onPointerDownCallback?.Invoke(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpCallback?.Invoke(eventData);
        }

        public virtual void OnPointerMove(PointerEventData eventData)
        {
            onPointerMoveCallback?.Invoke(eventData);
        }
    }
}