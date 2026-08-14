using System;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit.Scripts
{
    public class Stat
    {
        private string name;
        private float baseValue;
        private bool isDirty = true;
        private float cachedValue;
        private readonly List<StatModifier> statModifiers = new List<StatModifier>();

        // 값이 변경되었을 때벤트를 날려 UI나 외부 시스템에 알림
        public event Action<float> OnValueChanged;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                if (Math.Abs(baseValue - value) > 0.01f)
                {
                    baseValue = value;
                    SetDirty();
                }
            }
        }

        public float Value
        {
            get
            {
                if (isDirty)
                {
                    cachedValue = CalculateFinalValue();
                    isDirty = false;
                }

                return cachedValue;
            }
        }

        public Stat(string name,float baseValue)
        {
            this.name = name;
            this.baseValue = baseValue;
        }

        public void AddModifier(StatModifier mod)
        {
            SetDirty();
            statModifiers.Add(mod);
            statModifiers.Sort((a, b) => a.order.CompareTo(b.order));
        }

        public void RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                SetDirty();
            }
        }

        public void RemoveModifiersFromSource(object source)
        {
            bool removed = false;
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].source == source)
                {
                    statModifiers.RemoveAt(i);
                    removed = true;
                }
            }

            if (removed)
            {
                SetDirty();
            }
        }

        private void SetDirty()
        {
            isDirty = true;
            OnValueChanged?.Invoke(Value);
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.type == StatModifyType.Flat)
                {
                    finalValue += mod.value;
                }
                else if (mod.type == StatModifyType.PercentAdd)
                {
                    sumPercentAdd += mod.value;
                }
                else if (mod.type == StatModifyType.PercentMult)
                {
                    finalValue *= (1 + mod.value);
                }
            }

            finalValue *= (1 + sumPercentAdd);
            return (float)Math.Round(finalValue, 4); // 소수점 정리
        }

        public void ClearModifier()
        {
           statModifiers.Clear();
        }

        public override string ToString()
        {
            var finalValue = CalculateFinalValue();
            var result = $"{name} : {baseValue}(+{finalValue - baseValue})";
            return result;
        }
    }
}