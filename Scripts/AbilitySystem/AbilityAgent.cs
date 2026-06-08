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
        public List<AbilityEffect> effects = new List<AbilityEffect>();
        public IAbilityStatSet abilityStatSet;
        
        public void SetStats(IAbilityStatSet statSet)
        {
            abilityStatSet = statSet;
        }

        public T GetStats<T>() where T : IAbilityStatSet
        {
            return (T)abilityStatSet;
        }

        public void RemoveEffect(string id)
        {
            var effect = effects.Find(x => x.id == id);
            effects.Remove(effect);
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

        public void AddEffect(string id, AbilityProvider provider = null)
        {
            var effect = AbilityManager.CreateAbilityEffect(id);
            effect.Setup(this);
            effect.SetProvider(provider);
            effects.Add(effect);
        }
        
        public void ExecuteEffectsByTag<TProcessResult>(string tag, ref TProcessResult argumentData)
            where TProcessResult : IAbilityContext
        {
            var array = effects.ToArray();
            foreach (var effect in array)
            {
                if (effect.tags.Contains(tag))
                {
                    effect.TryExecute(argumentData);
                }
            }
           
        }

        public void ExecuteEffectById<TProcessResult>(string id, ref TProcessResult argumentData)
            where TProcessResult : IAbilityContext
        {
             effects.Find(x=>x.id == id).TryExecute(argumentData);
        }

        public void RegisterPreExecutionCallback(string id, Action<IAbilityContext> action)
        {
            effects.Find(x=>x.id == id).RegisterPreExecutionCallback(action);
        }
        
        public void RegisterPostExecutionCallback(string id, Action<IAbilityContext> action)
        {
            effects.Find(x=>x.id == id).RegisterPostExecutionCallback(action);
        }
    }
}