using UnityEngine;

namespace KCoreKit
{
    public abstract class LocalizedComponentBase :  MonoBehaviour
    {  
        
        protected LocalizationManager localizationManager => LocalizationManager.GetInstance();

        public void Awake()
        {
            LocalizationManager.onChange += OnChange;
        }

        public abstract void OnChange();
    }
}