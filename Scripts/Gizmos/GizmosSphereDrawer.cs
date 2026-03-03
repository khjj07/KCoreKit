using UnityEngine;

namespace KCoreKit
{
    public class GizmosSphereDrawer : MonoBehaviour
    {
        [SerializeField] private Color _color = Color.red;
        [SerializeField] public float size = 1.0f;

        [SerializeField] private bool _wireFrame;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = _color;
            if (_wireFrame)
                UnityEngine.Gizmos.DrawWireSphere(transform.position, size);
            else
                UnityEngine.Gizmos.DrawSphere(transform.position, size);
        }
#endif
    }
}