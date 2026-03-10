using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KCoreKit
{
    public class AbilityAgent : MonoBehaviour
    {
        private PlayerInput _playerInput;
        public List<AbilityEffectBase> effects = new List<AbilityEffectBase>();
        public IAbilityStats baseAbilityStats;
        public IAbilityStats currentAbilityStats;

        public void Awake()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();
        }
        
        public void SetStats(IAbilityStats stats)
        {
            baseAbilityStats = stats;
            currentAbilityStats = stats;
        }
        
        public void AddEffect(AbilityEffectBase effect)
        {
            DeactivateAllEffect();
            effects.Add(effect);
            _playerInput.onActionTriggered += effect.OnActionTriggered;
            ReorderEffect();
            ResetAllStats();
            ActivateAllEffect();
        }

        public void RemoveEffect(AbilityEffectBase effect)
        {
            DeactivateAllEffect();
            effects.Remove(effect);
            _playerInput.onActionTriggered -= effect.OnActionTriggered;
            ResetAllStats();
            ActivateAllEffect();
        }
        
        public AbilityEffectBase GetEffect(string name)
        {
            return effects.Find(x => x.name == name);
        }

        public List<AbilityEffectBase> GetEffects(string tag)
        {
            return effects.FindAll(x => x.tag.Contains(tag));
        }

        private void ReorderEffect()
        {
            effects.Sort((a, b) => a.order - b.order);
        }

        public void ResetAllStats()
        {
            foreach (var stat in baseAbilityStats.Get())
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
     
    }
}