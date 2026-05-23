
using UnityEngine;

namespace KCoreKit
{
    public interface IAbilityProvider
    {
        public Transform GetTransform();
        public void SetProperty(string key, string value);
        public string GetProperty(string key);
        
        public bool HasProperty(string key);
    }
}