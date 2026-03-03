using UnityEngine;

namespace KCoreKit
{
    public abstract class DependentComponent<T> : MonoBehaviour
    {
        public abstract void Initialize(T dependedObject);
    }
}