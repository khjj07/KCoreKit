using UnityEngine;

namespace KCoreKit
{
    public class GizmosMeshDrawer : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Color _color = Color.red;
        [SerializeField] private Vector3 _scale = Vector3.one;
        [SerializeField] private bool _wireFrame;
        // Update is called once per frame
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var scale = Vector3.zero;
            scale.x = transform.localScale.x * _scale.x;
            scale.y = transform.localScale.y * _scale.y;
            scale.z = transform.localScale.z * _scale.z;
            UnityEngine.Gizmos.color = _color;
            if (_wireFrame)
                UnityEngine.Gizmos.DrawWireMesh(_mesh, transform.position, transform.rotation, scale);
            else
                UnityEngine.Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, scale);
        }
#endif
    }
}