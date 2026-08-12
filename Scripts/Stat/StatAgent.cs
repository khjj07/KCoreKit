using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit.Scripts
{
    public class StatAgent : MonoBehaviour
    {
        private Dictionary<string, Stat> _stats = new Dictionary<string, Stat>();


        public void AddStat(string key, float baseValue = 0)
        {
            _stats.Add(key,new Stat(baseValue));
        }
        public void SetBaseValue(string key, float baseValue)
        {
            _stats[key].BaseValue = baseValue;
        }
        public Stat GetStat(string key)
        {
            return _stats[key];
        }

        public float GetFinalValue(string key)
        {
            return _stats[key].Value;
        }


        public Dictionary<string, Stat> GetAllStats()
        {
            return _stats;
        }

        public void ClearStatModifier(string key)
        {
            _stats[key].ClearModifier();
        }
    }
}