using System;
using DG.Tweening;

namespace KCoreKit
{
    [Serializable]
    public struct TransformTweenData
    {
        public Ease ease;
        public float duration;
        public float delay;
        public TransformData transform;
    }
}