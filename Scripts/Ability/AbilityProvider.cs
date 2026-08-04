
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public class AbilityProvider : MonoBehaviour
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        
        public void SetProperty(string key, string value)
        {
            if (properties.ContainsKey(key))
            {
                properties[key] = value;
            }
            else
            {
                properties.Add(key, value);
            }
        }

        public string GetPropertyString(string key)
        {
            return properties[key];
        }
        
        public float GetPropertyFloat(string key)
        {
            return float.Parse(properties[key]);
        }

        public int GetPropertyInt(string key)
        {
            return int.Parse(properties[key]);
        }

        public bool GetPropertyBool(string key)
        {
            return bool.Parse(properties[key]);
        }

        public bool HasProperty(string key)
        {
            return properties.ContainsKey(key);
        }

        public Dictionary<string, string> GetProperties()
        {
            return properties;
        }
    }
}