using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KCoreKit
{
    public class AbilityAgent : MonoBehaviour
    {
        public Dictionary<string, AbilityEffect> effects = new Dictionary<string, AbilityEffect>();
        public IAbilityStatSet abilityStatSet;
        
        public void SetStats(IAbilityStatSet statSet)
        {
            abilityStatSet = statSet;
        }

        public T GetStats<T>() where T : IAbilityStatSet
        {
            return (T)abilityStatSet;
        }

        public void RemoveEffect(string instanceId)
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

        public void AddEffect(string instanceId, string id, IAbilityProvider provider)
        {
            var effect = AbilityManager.CreateAbilityEffect(id,provider);
            effects.Add(instanceId, effect);
            effect.Setup(this);
        }
        
        public void ExecuteEffects<TProcessResult>(string tag, ref TProcessResult argumentData)
            where TProcessResult : IAbilityContext
        {
            foreach (var effect in effects.Values.ToList())
            {
                if (effect.tags.Contains(tag))
                {
                    effect.TryExecute(argumentData);
                }
            }
           
        }

        public void ExecuteEffect<TProcessResult>(string instanceId, ref TProcessResult argumentData)
            where TProcessResult : IAbilityContext
        {
             effects[instanceId].TryExecute(argumentData);
        }

        public void RegisterExecutionCallback(string instanceId, Action<IAbilityContext> action)
        {
            effects[instanceId].RegisterExecutionCallback(action);
        }
    }
}