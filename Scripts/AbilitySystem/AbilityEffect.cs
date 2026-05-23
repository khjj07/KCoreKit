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
        public IAbilityProvider provider;
        public AbilityAgent owner;
        public string[] tags;
        private bool _isActive;
        
        private List<List<MethodInfo>> orConditionMethods;
        private List<MethodInfo> currentAndConditionGroup;
        private List<Dictionary<string, string>> conditionProperties;
        
        private List<MethodInfo> actionMethods;
        private List<Dictionary<string, string>> actionProperties;
        private Action<IAbilityContext> _callback;

        public AbilityEffect(string id,IAbilityProvider provider, List<string> tags)
        {
            this.id = id;
            this.provider = provider;
            this.tags = tags.ToArray();
            orConditionMethods = new List<List<MethodInfo>>();
            conditionProperties = new List<Dictionary<string, string>>();
            actionMethods = new List<MethodInfo>();
            actionProperties = new List<Dictionary<string, string>>();
        }
        
        public void Setup(AbilityAgent owner)
        {
            this.owner = owner;
        }
        
        public void AddNewOrConditionGroup()
        {
            currentAndConditionGroup = new List<MethodInfo>();
            orConditionMethods.Add(currentAndConditionGroup);
        }

        public void BindAndCondition(MethodInfo condition, Dictionary<string, string> prop)
        {
            currentAndConditionGroup.Add(condition);
            conditionProperties.Add(prop);
        }

        public void BindAction(MethodInfo action, Dictionary<string, string> prop)
        {
            actionMethods.Add(action);
            actionProperties.Add(prop);
        }


        public void InvokeAction<TProcessResult>(ref TProcessResult argumentData)
        {
            for (int i = 0; i < actionMethods.Count; i++)
            {
               actionMethods[i].Invoke(null, new object[] { this, actionProperties[i], argumentData });
            }
        }


        public bool EvaluateCondition<TArgumentData>(ref TArgumentData argumentData)
        {
            var result = false;
            for (int i = 0; i < orConditionMethods.Count; i++)
            {
                var andConditionResult = true;
                foreach (var andCondition in orConditionMethods[i])
                {
                    andConditionResult &= (bool)andCondition.Invoke(null, new object[] { this, conditionProperties[i], argumentData });
                }

                result |= andConditionResult;
            }

            return result;
        }


        public bool TryExecute<TProcessResult>(TProcessResult result) where TProcessResult : IAbilityContext
        {
            if (EvaluateCondition(ref result))
            {
                InvokeAction(ref result);
                _callback?.Invoke(result);
                return true;
            }
            return false;
        }

        public void RegisterExecutionCallback(Action<IAbilityContext> action)
        {
            _callback += action;
        }
    }
}