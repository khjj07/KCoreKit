using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{

    public class TooltipWidget : WidgetBase
    {
        public string id;
        [SerializeField] private RectTransform root;
        private Dictionary<string,TooltipTextWidget> textDictionary;
        private Dictionary<string,TooltipImageWidget> imageDictionary;

        public void Awake()
        {
            textDictionary = GetComponentsInChildren<TooltipTextWidget>(true).ToDictionary(x => x.key);
            imageDictionary = GetComponentsInChildren<TooltipImageWidget>(true).ToDictionary(x => x.key);
        }

        public void OnShow(TooltipContext context)
        {
            UpdatePosition(context);

            foreach (var text in context.textDictionary)
            {
                textDictionary[text.Key].SetText(text.Value);
            }
            
            foreach (var sprite in context.spriteDictionary)
            {
                imageDictionary[sprite.Key].SetSprite(sprite.Value);
            }

        }

        public void OnUpdate(TooltipContext context)
        {
            UpdatePosition(context);
        }

        private void UpdatePosition(TooltipContext context)
        {
            var canvasRect = (RectTransform)canvas.transform;

            // Overlay 캔버스에는 카메라를 넘기면 안 되고, 그 외에는 캔버스에 지정된 카메라를 써야 한다.
            // Camera.main 이 캔버스의 렌더 카메라와 같다는 보장은 없다.
            var canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            Vector2 screenPoint;
            if (context.screenSpace)
            {
                // tooltipPosition 이 스크린 픽셀. offset 도 픽셀 단위로 더한다.
                screenPoint = new Vector2(context.tooltipPosition.x, context.tooltipPosition.y) + context.offset;
            }
            else
            {
                // tooltipPosition 이 월드 좌표. offset 은 월드 단위로 먼저 더한 뒤 스크린으로 변환한다.
                var worldCamera = canvas.worldCamera ? canvas.worldCamera : Camera.main;
                screenPoint = RectTransformUtility.WorldToScreenPoint(
                    worldCamera, context.tooltipPosition + (Vector3)context.offset);
            }

            // ScreenPointToLocalPointInRectangle 의 결과는 캔버스 피벗 기준 로컬 좌표라,
            // root 의 앵커가 중앙이 아니면 원점이 어긋난다(이 프로젝트의 툴팁은 앵커가 좌하단).
            // 월드 좌표로 받아 position 에 넣으면 앵커/피벗과 무관하게 항상 정확하다.
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    canvasRect, screenPoint, canvasCamera, out var worldPoint))
            {
                root.position = worldPoint;
            }
        }
    }
}