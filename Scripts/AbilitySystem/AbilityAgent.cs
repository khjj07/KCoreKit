using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KCoreKit
{
    public class AbilityAgent : MonoBehaviour
    {
        public List<AbilityEffectBase> effects = new List<AbilityEffectBase>();
        public IAbilityStatSet abilityStatSet;
        

        public void SetStats(IAbilityStatSet statSet)
        {
            abilityStatSet = statSet;
        }

        public T GetStats<T>() where T : IAbilityStatSet
        {
            return (T)abilityStatSet;
        }
        
        public void AddEffect(AbilityEffectBase effect)
        {
            effects.Add(effect);
            effect.Setup(this);
        }

        public void RemoveEffect(AbilityEffectBase effect)
        {
            effects.Remove(effect);
            effect.Deactivate();
        }

        public AbilityEffectBase GetEffectByName(string name)
        {
            return effects.Find(x => x.name == name);
        }
        
        public  List<AbilityEffectBase> GetEffectsByName(string name)
        {
            return effects.FindAll(x => x.name == name);
        }
        
        public List<AbilityEffectBase> GetEffectsByTag(string tag)
        {
            return effects.FindAll(x => x.tag.Contains(tag));
        }

        private void ReorderEffect()
        {
            effects.Sort((a, b) => a.order - b.order);
        }

        public void ResetAllStats()
        {
            foreach (var stat in abilityStatSet.Get())
            {
                stat.Reset();
            }
        }

        public void ActivateAllEffect()
        {
            foreach (var effect in effects)
            {
                effect.Activate();
            }
        }

        public void DeactivateAllEffect()
        {
            foreach (var effect in effects)
            {
                effect.Deactivate();
            }
        }

        public void ClearEffect()
        {
            DeactivateAllEffect();
            effects.Clear();
            ResetAllStats();
        }
    }
}