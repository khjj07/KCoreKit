using System.Collections.Generic;

namespace KCoreKit
{
    public class GPUInstancingManager : Singleton<GPUInstancingManager>
    {
        private static Dictionary<string, InstancingGroupBase> instancingGroups =
            new Dictionary<string, InstancingGroupBase>();
        
        public static bool ContainsGroup(string key)
        {
           return instancingGroups.ContainsKey(key);
        }
        public static void TryAddGroup(string key, InstancingGroupBase group)
        {
            instancingGroups.TryAdd(key, group);
        }

        public static void AddInstance(string key, InstanceBase instance)
        {
            if (instancingGroups.ContainsKey(key))
            {
                instancingGroups[key].AddDrawData(instance);
                UnityEngine.Debug.Log("instance added");
            }
        }


        public static void RemoveInstance(string key, InstanceBase instance)
        {
            instancingGroups[key].RemoveDrawData(instance);
        }


        public void Render()
        {
            foreach (var group in instancingGroups)
            {
                group.Value.Render();
            }
        }

        public static void UpdateDrawData()
        {
            foreach (var group in instancingGroups)
            {
                group.Value.UpdateDrawBuffers();
            }
        }
        
        public static void UpdateDrawData(string key, InstanceBase instance)
        {
            instancingGroups[key].UpdateDrawBuffers();
        }


        public void OnApplicationQuit()
        {
            foreach (var group in instancingGroups)
            {
                group.Value.Release();
            }
        }

        public static void Release()
        {
            foreach (var group in instancingGroups)
            {
                group.Value.Release();
            }
            instancingGroups.Clear();
        }

        public static void UpdateCustomData(string tag, object data = null)
        {
            foreach (var group in instancingGroups)
            {
                group.Value.UpdateCustomData(tag, data);
            }
        }
    }
}