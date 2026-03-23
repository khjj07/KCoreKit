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
        public float cooldown;
        public string[] tag;

        private bool _isActive;

        public AbilityEffectBase(string name, int order, float cooldown, string[] tag)
        {
            this.name = name;
            this.cooldown = cooldown;
            this.order = order;
            this.tag = tag;
        }

        public void Setup(AbilityAgent owner)
        {
            this.owner = owner;
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