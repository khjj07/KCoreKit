using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{
    public class SpriteRenderGroup : MonoBehaviour
    {
        private List<SpriteRenderer> _spriteRenderers;

        public void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        }

        public void SetFade(float alpha)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                var color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
    }
}