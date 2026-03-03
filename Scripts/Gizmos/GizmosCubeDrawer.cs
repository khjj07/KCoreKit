using UnityEngine;

namespace KCoreKit
{
    public class GizmosCubeDrawer : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] public Vector3 size = Vector3.one;
        [SerializeField] public Vector3 offset = Vector3.zero;

        [SerializeField] private bool _wireFrame;
        // Update is called once per frame
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var scale = Vector3.zero;
            scale.x = transform.localScale.x * size.x;
            scale.y = transform.localScale.y * size.y;
            scale.z = transform.localScale.z * size.z;
            UnityEngine.Gizmos.color = color;
            if (_wireFrame)
                UnityEngine.Gizmos.DrawWireCube(transform.position+offset, scale);
            else
                UnityEngine.Gizmos.DrawCube(transform.position+offset, scale);
        }
#endif
    }
}