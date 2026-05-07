using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KCoreKit
{
    public class AbilityAgent : MonoBehaviour
    {
        public Dictionary<int, AbilityEffect> effects = new Dictionary<int, AbilityEffect>();
        public IAbilityStatSet abilityStatSet;


        public void SetStats(IAbilityStatSet statSet)
        {
            abilityStatSet = statSet;
        }

        public T GetStats<T>() where T : IAbilityStatSet
        {
            return (T)abilityStatSet;
        }

        public void RemoveEffect(int instanceId)
        {
            effects.Remove(instanceId);
        }

        public void ResetAllStats()
        {
            foreach (var stat in abilityStatSet.Get())
            {
                stat.Reset();
            }
        }

        public void ClearEffect()
        {
            effects.Clear();
            ResetAllStats();
        }

        public void AddEffect(int instanceId, string id)
        {
            var effect = AbilityManager.CreateAbilityEffect(id);
            effects.Add(instanceId, effect);
            effect.Setup(this);
        }

        public bool ExecuteEffect<TProcessResult>(int instanceId, ref TProcessResult argumentData)
            where TProcessResult : class
        {
            return effects[instanceId].TryExecute(argumentData);
        }
    }
}