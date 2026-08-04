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
  
        public void RemoveEffect(string id)
        {
            var effect = effects.Find(x => x.id == id);
            effects.Remove(effect);
        }

        public void ClearEffect()
        {
            effects.Clear();
        }

        public void AddEffect(string id, object source = null)
        {
            var effect = AbilitySystem.CreateAbilityEffect(id);
            effect.Setup(this,source);
            effects.Add(effect);
        }

        public void RemoveEffectFromSource(object source)
        {
            bool removed = false;
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                if (effects[i].source == source)
                {
                    effects.RemoveAt(i);
                    removed = true;
                }
            }
        }
        
        public void ExecuteEffectsByTag<TAbilityContext>(string tag, ref TAbilityContext argumentData)
            where TAbilityContext : IAbilityContext
        {
            var array = effects.ToArray();
            foreach (var effect in array)
            {
                if (effect.tags.Contains(tag))
                {
                    effect.TryExecute(ref argumentData);
                }
            }

        }

        public void ExecuteEffectById<TAbilityContext>(string id, ref TAbilityContext argumentData)
            where TAbilityContext : IAbilityContext
        {
            effects.Find(x => x.id == id).TryExecute(ref argumentData);
        }

        public void RegisterPreExecutionCallback(string id, Action<IAbilityContext> action)
        {
            effects.Find(x => x.id == id).RegisterPreExecutionAction(action);
        }

        public void RegisterPostExecutionCallback(string id, Action<IAbilityContext> action)
        {
            effects.Find(x => x.id == id).RegisterPostExecutionAction(action);
        }
        
       
    }
}