using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    public class SpriteRenderGroup : MonoBehaviour
    {
        
        private List<SpriteRenderer> _spriteRenderers;
        private bool _shown;

        public void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        }

        public void Fade(float alpha,float duration)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.DOFade(alpha, duration);
            }
        }
    }
}