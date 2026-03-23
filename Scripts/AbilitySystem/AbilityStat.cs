using System;

namespace KCoreKit
{

    [Serializable]
    public class AbilityStat
    {
        public readonly string name;
        public readonly float baseValue;
        public float currentValue;
        public AbilityStat()
        {
           
        }
        public AbilityStat(string name, float baseValue = 0)
        {
            this.name = name;
            this.baseValue = baseValue;
            this.currentValue = baseValue;
        }

        public string GetName()
        {
            return name;
        }
        
        public void SetValue(float value)
        {
            currentValue = value;
        }
        
        public float GetValue()
        {
            return currentValue;
        }
        
        public void Reset()
        {
            currentValue = baseValue;
        }

        public void Add(float value)
        {
            currentValue += value;
        }
        
        public void Substract(float value)
        {
            currentValue -= value;
        }
        
        public void Multiply(float value)
        {
            currentValue *= value;
        }
    }
}