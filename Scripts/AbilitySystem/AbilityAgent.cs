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
        public IAbilityStats abilityStats;

        public void Awake()
        {
            _playerInput = FindAnyObjectByType<PlayerInput>();
        }

        public void SetStats(IAbilityStats stats)
        {
            abilityStats = stats;
        }

        public T GetStats<T>() where T : IAbilityStats
        {
            return (T)abilityStats;
        }


        public void AddEffect(AbilityEffectBase effect)
        {
            DeactivateAllEffect();
            effects.Add(effect);
            effect.Setup(this);
            if (_playerInput)
            {
                _playerInput.onActionTriggered += effect.OnActionTriggered;
            }

            ReorderEffect();
            ResetAllStats();
            ActivateAllEffect();
        }

        public void RemoveEffect(AbilityEffectBase effect)
        {
            DeactivateAllEffect();
            effects.Remove(effect);
            if (_playerInput)
            {
                _playerInput.onActionTriggered -= effect.OnActionTriggered;
            }

            ResetAllStats();
            ActivateAllEffect();
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
            foreach (var stat in abilityStats.Get())
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
            foreach (var effect in effects)
            {
                if (_playerInput)
                {
                    _playerInput.onActionTriggered -= effect.OnActionTriggered;
                }
            }

            effects.Clear();
            ResetAllStats();
        }
    }
}