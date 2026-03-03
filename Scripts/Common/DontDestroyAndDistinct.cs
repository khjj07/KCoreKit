using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public class DontDestroyAndDistinct : MonoBehaviour
    {
        private static Dictionary<string,DontDestroyAndDistinct> _instance = new Dictionary<string, DontDestroyAndDistinct>();

        private void Awake()
        {
            if (!_instance.ContainsKey(gameObject.name))
            {
                 _instance.Add(gameObject.name,this);
                 DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
