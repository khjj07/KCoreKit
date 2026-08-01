using UnityEngine;

namespace KCoreKit
{
    public class AbilityScheduler
    {
        public float interval;
        public float _elapsedTime;
        public string abilityId;
        public string id;
        private AbilityAgent agent;
        private IAbilityContext context;

        public AbilityScheduler(AbilityAgent agent, string id, float interval, string abilityId, IAbilityContext context)
        {
            this.agent = agent;
            this.id = id;
            this.interval = interval;
            this.abilityId = abilityId;
            this.context = context;
        }
        
        public void OnUpdate()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > interval)
            {
                InvokeAbility();
                _elapsedTime = 0;
            }
        }
        

        private void InvokeAbility()
        {
           agent.ExecuteEffectById(abilityId,ref context);
        }
    }
}