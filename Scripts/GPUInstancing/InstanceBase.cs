using UnityEngine;

namespace KCoreKit
{
    public abstract class InstanceBase : MonoBehaviour
    {
        [ReadOnly] 
        public string guid;
    }
}