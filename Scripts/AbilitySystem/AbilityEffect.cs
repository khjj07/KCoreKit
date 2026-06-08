using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;

namespace KCoreKit
{

    public class AbilityEffect
    {
        public string id;
        public AbilityProvider provider;
        public AbilityAgent owner;
        public string[] tags;
        private bool _isActive;
        
        private List<List<MethodInfo>> orConditionMethods;
        private List<MethodInfo> currentAndConditionGroup;
        private List<AbilityPropertySet> conditionProperties;
        
        private List<MethodInfo> actionMethods;
        private List<AbilityPropertySet> actionProperties;
        private Action<IAbilityContext> _onPreExecute;
        private Action<IAbilityContext> _onPostExecute;

        public AbilityEffect(string id, List<string> tags)
        {
            this.id = id;
            this.tags = tags.ToArray();
            orConditionMethods = new List<List<MethodInfo>>();
            conditionProperties = new List<AbilityPropertySet>();
            actionMethods = new List<MethodInfo>();
            actionProperties = new List<AbilityPropertySet>();
        }
        
        public void Setup(AbilityAgent owner)
        {
            this.owner = owner;
        }

        public void SetProvider(AbilityProvider provider)
        {
            this.provider = provider;
        }
        
        public void AddNewOrConditionGroup()
        {
            currentAndConditionGroup = new List<MethodInfo>();
            orConditionMethods.Add(currentAndConditionGroup);
        }

        public void BindAndCondition(MethodInfo condition, Dictionary<string, string> prop)
        {
            currentAndConditionGroup.Add(condition);
            conditionProperties.Add(new AbilityPropertySet(this,prop));
        }

        public void BindAction(MethodInfo action, Dictionary<string, string> prop)
        {
            actionMethods.Add(action);
            actionProperties.Add(new AbilityPropertySet(this,prop));
        }


        public void InvokeAction<TAbilityContext>(ref TAbilityContext context) where TAbilityContext : IAbilityContext
        {
            for (int i = 0; i < actionMethods.Count; i++)
            {
               actionMethods[i]?.Invoke(null, new object[] { this, actionProperties[i], context });
            }
        }


        public bool EvaluateCondition<TAbilityContext>(ref TAbilityContext context) where TAbilityContext : IAbilityContext
        {
            var result = false;
            for (int i = 0; i < orConditionMethods.Count; i++)
            {
                var andConditionResult = true;
                foreach (var andCondition in orConditionMethods[i])
                {
                    andConditionResult &= (bool)andCondition.Invoke(null, new object[] { this, conditionProperties[i], context });
                }

                result |= andConditionResult;
            }

            return result;
        }


        public bool TryExecute<TAbilityContext>(TAbilityContext result) where TAbilityContext : IAbilityContext
        {
            if (EvaluateCondition(ref result))
            {
                _onPreExecute?.Invoke(result);
                InvokeAction(ref result);
                _onPostExecute?.Invoke(result);
                return true;
            }
            return false;
        }

        public void RegisterPreExecutionCallback(Action<IAbilityContext> action)
        {
            _onPreExecute += action;
        }
        
        public void RegisterPostExecutionCallback(Action<IAbilityContext> action)
        {
            _onPostExecute += action;
        }
    }
}