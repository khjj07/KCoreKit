using System;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
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
    public class WidgetBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [HideInInspector]
        public RectTransform rectTransform
        {
            get
            {
                if (!_rectTransform)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        [HideInInspector]
        public CanvasGroup canvasGroup
        {
            get
            {
                if (!_canvasGroup)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        [HideInInspector]
        public Canvas canvas
        {
            get
            {
                if (!_canvas)
                {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }


        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        public bool isShown => gameObject.activeSelf;

        public Action<PointerEventData> onPointerClickAction;
        public Action<PointerEventData> onPointerEnterAction;
        public Action<PointerEventData> onPointerExitAction;
        public Action<PointerEventData> onPointerDownAction;
        public Action<PointerEventData> onPointerUpAction;
        public Action<PointerEventData> onPointerMoveAction;

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetCanvas(Canvas canvas, bool worldPositionStays = false)
        {
            _canvas = canvas;
            transform.SetParent(_canvas.transform, worldPositionStays);
        }

        public void SetParent(Transform parent, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onPointerClickAction?.Invoke(eventData);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnterAction?.Invoke(eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            onPointerExitAction?.Invoke(eventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            onPointerDownAction?.Invoke(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            onPointerUpAction?.Invoke(eventData);
        }

        public virtual void OnPointerMove(PointerEventData eventData)
        {
            onPointerMoveAction?.Invoke(eventData);
        }
    }
}