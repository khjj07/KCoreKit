using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    [CreateAssetMenu(fileName = "GlobalPrefabManager", menuName = "Default/Common/GlobalPrefabManager")]
    public class GlobalPrefabFactory : SingletonAsset<GlobalPrefabFactory>
    {
      
        
        public List<GameObject> prefabs;
        public static T Create<T>()
        { 
            var instance = GetInstance();
            var data = instance.prefabs.Find(x => x.GetComponent<T>() != null);
            return Instantiate(data).GetComponent<T>();
        }
        
        public static T CachePrefab<T>()
        {
            var instance = GetInstance();
            var data = instance.prefabs.Find(x => x.GetComponent<T>() != null);
            return data.GetComponent<T>();
        }
    }
}