using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace KCoreKit
{
    public class TweenAnimationSequenceConvertor : MonoBehaviour
    {
        private Sequence _sequence;
        public void Awake()
        {
            _sequence = DOTween.Sequence();
            
            var animationComponents = GetComponents<ABSAnimationComponent>();
            foreach (var component in animationComponents)
            {
                _sequence.AppendCallback(component.DOPlay);
            }
        }

        public Sequence GetSequence()
        {
            return _sequence;
        }

        public Sequence Play()
        {
            return _sequence.Play();
        }
    }
}