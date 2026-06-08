using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public class AbilityPropertySet
    {
        private AbilityEffect _effect;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public AbilityPropertySet(AbilityEffect effect, Dictionary<string, string> prop)
        {
            _effect = effect;
            _properties = prop;
        }

        public void AddProperty(string key, string value)
        {
            _properties.Add(key, value);
        }

        public string GetPropertyString(string key)
        {
            var provider = _effect.provider;
            if (provider.HasProperty(_properties[key]))
            {
                var providerValue = provider.GetPropertyString(_properties[key]);
                return providerValue;
            }

            return _properties[key];
        }

        public float GetPropertyFloat(string key, float defaultValue = 0)
        {
            var provider = _effect.provider;
            var isNumber = float.TryParse(_properties[key], out var number);
            if (isNumber)
            {
                return number;
            }
            if (provider.HasProperty(_properties[key]))
            {
             
                var providerValue = provider.GetPropertyFloat(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }
        
        public int GetPropertyInt(string key, int defaultValue = 0)
        {
            var provider = _effect.provider;
            var isNumber = int.TryParse(_properties[key], out var number);
            if (isNumber)
            {
                return number;
            }
            if (provider.HasProperty(_properties[key]))
            {
                var providerValue = provider.GetPropertyInt(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }
        
        public bool GetPropertyBool(string key, bool defaultValue = false)
        {
            var provider = _effect.provider;
            var isBool = bool.TryParse(_properties[key], out var boolean);
            if (isBool)
            {
                return boolean;
            }
            
            if (provider.HasProperty(_properties[key]))
            {
                var providerValue = provider.GetPropertyBool(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }

        
        public Dictionary<string, string>.KeyCollection GetKeys()
        {
            return _properties.Keys;
        }
    }
}