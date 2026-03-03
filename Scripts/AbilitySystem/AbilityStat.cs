using System;

namespace KCoreKit
{

    [Serializable]
    public class AbilityStat
    {
        public readonly float baseValue;
        public float currentValue;
        public AbilityStat()
        {
           
        }
        public AbilityStat(float baseValue = 0)
        {
            this.baseValue = baseValue;
            this.currentValue = baseValue;
        }
        
        public void Set(float value)
        {
            currentValue = value;
        }
        
        public float Get()
        {
            return currentValue;
        }
        
        public void Reset()
        {
            currentValue = baseValue;
        }
        
    }
}