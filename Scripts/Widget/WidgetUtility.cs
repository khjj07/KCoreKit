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
    }
}