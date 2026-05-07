using System;
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
    }
}