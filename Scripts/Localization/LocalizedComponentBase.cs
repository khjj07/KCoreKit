using UnityEngine;

namespace KCoreKit
{
    public abstract class LocalizedComponentBase :  MonoBehaviour
    {  
        public virtual void Awake()
        {
            LocalizationManager.onChange += OnChange;
        }

        public void OnDestroy()
        {
            LocalizationManager.onChange -= OnChange;
        }

        public abstract void OnChange();
    }
}