using UnityEngine;

namespace KCoreKit
{
    public static class WidgetUtility
    {
        public static Vector2 WorldPositionToScreenAnchoredPosition(Camera camera, Canvas canvas, Vector3 worldPosition)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform, screenPoint, camera, out var localPoint);
            return localPoint;
        }
        
        public static Vector2 WorldPositionToScreenPosition(Camera camera, Vector3 worldPosition)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            return screenPoint;
        }
        
        public static bool IsOverlapping(RectTransform a, RectTransform b)
        {
            if (a == null || b == null) return false;

            Vector3[] cornersA = new Vector3[4];
            Vector3[] cornersB = new Vector3[4];

            a.GetWorldCorners(cornersA);
            b.GetWorldCorners(cornersB);

            // 좌측 하단(0)과 우측 상단(2) 기준으로 Rect 생성
            Rect rect1 = new Rect(cornersA[0], cornersA[2] - cornersA[0]);
            Rect rect2 = new Rect(cornersB[0], cornersB[2] - cornersB[0]);

            return rect1.Overlaps(rect2);
        }
    }
}