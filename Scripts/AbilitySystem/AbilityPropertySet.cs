using System.Collections;
using System.Collections.Generic;

namespace KCoreKit
{
    public class AbilityPropertySet
    {
        private AbilityProvider _provider;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public AbilityPropertySet(AbilityProvider provider, Dictionary<string, string> prop)
        {
            _provider = provider;
            _properties = prop;
        }

        public void AddProperty(string key, string value)
        {
            _properties.Add(key, value);
        }

        public string GetPropertyString(string key,string defaultValue = "")
        {
            if (_provider.HasProperty(_properties[key]))
            {
                var providerValue = _provider.GetPropertyString(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }

        public float GetPropertyFloat(string key, float defaultValue = 0)
        {
            var isNumber = float.TryParse(_properties[key], out var number);
            if (isNumber)
            {
                return number;
            }
            
            if (_provider.HasProperty(_properties[key]))
            {
                var providerValue = _provider.GetPropertyFloat(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }
        
        public int GetPropertyInt(string key, int defaultValue = 0)
        {
            var isNumber = int.TryParse(_properties[key], out var number);
            if (isNumber)
            {
                return number;
            }
            
            if (_provider.HasProperty(_properties[key]))
            {
                var providerValue = _provider.GetPropertyInt(_properties[key]);
                return providerValue;
            }

            return defaultValue;
        }
        
        public bool GetPropertyBool(string key, bool defaultValue = false)
        {
            var isBool = bool.TryParse(_properties[key], out var boolean);
            if (isBool)
            {
                return boolean;
            }
            
            if (_provider.HasProperty(_properties[key]))
            {
                var providerValue = _provider.GetPropertyBool(_properties[key]);
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