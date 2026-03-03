using UnityEngine;

namespace KCoreKit
{
    public class GizmosExtension
    {
        public static void DrawArrow(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 30.0f)
        {
            UnityEngine.Gizmos.color = color;
            UnityEngine.Gizmos.DrawLine(from, to);

            var right = Quaternion.LookRotation(to - from) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) *
                        new Vector3(0, 0, 1);
            var left = Quaternion.LookRotation(to - from) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) *
                       new Vector3(0, 0, 1);
            UnityEngine.Gizmos.DrawRay(to, right * arrowHeadLength);
            UnityEngine.Gizmos.DrawRay(to, left * arrowHeadLength);
        }
    }
}
