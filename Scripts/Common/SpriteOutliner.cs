using UnityEngine;

namespace KCoreKit
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutliner : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        public void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetEnable(bool value)
        {
            _renderer.material.SetFloat("_OutlineEnabled",value ? 1 : 0);
        }

        public void SetColor(Color color)
        {
            _renderer.material.SetColor("_SolidOutline",color);
        }
    }
}
