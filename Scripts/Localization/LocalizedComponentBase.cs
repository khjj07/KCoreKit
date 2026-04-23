using UnityEngine;

namespace KCoreKit
{
    public abstract class LocalizedComponentBase :  MonoBehaviour
    {  
        
        protected LocalizationManager localizationManager => LocalizationManager.GetInstance();

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