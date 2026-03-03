using UnityEngine.InputSystem;

namespace KCoreKit
{
    public enum AbilityEffectState
    {
        Success,
        Running,
        Failure,
        Cancel,
    }

    public abstract class AbilityEffectBase
    {
        public string name;
        public AbilityAgent owner;
        public int order;
        public int cooldown;
        public string[] tag;

        private bool _isActive;

        public AbilityEffectBase(string name, AbilityAgent owner, int order, int cooldown, string[] tag)
        {
            this.name = name;
            this.owner = owner;
            this.cooldown = cooldown;
            this.order = order;
            this.tag = tag;
        }
        
        protected virtual bool CanActiveEffect()
        {
            return !_isActive;
        }

        protected virtual AbilityEffectState OnActivateEffect()
        {
            return AbilityEffectState.Success;
        }

        protected virtual AbilityEffectState OnDeactivateEffect()
        {
            return AbilityEffectState.Success;
        }

        public virtual void OnActionTriggered(InputAction.CallbackContext context)
        {
            
        }

        public void Activate()
        {
            if (CanActiveEffect())
            {
                _isActive = true;
                OnActivateEffect();
            }
        }

        public void Deactivate()
        {
            OnDeactivateEffect();
            _isActive = false;
        }

        public bool IsActivate()
        {
            return _isActive;
        }
    }
}