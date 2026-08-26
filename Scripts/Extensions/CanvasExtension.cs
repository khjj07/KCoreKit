using UnityEngine;

namespace KCoreKit
{
    public static class CanvasExtension
    {
        public static void Open(this Canvas canvas)
        {
            canvas.gameObject.SetActive(true);
        }
        public static void Close(this Canvas canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}