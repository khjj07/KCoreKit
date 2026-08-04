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
        public object source;
        public AbilityAgent owner;
        public string[] tags;
        private bool _isActive;
        
        private List<List<MethodInfo>> orConditionMethods;
        private List<MethodInfo> currentAndConditionGroup;
        private List<AbilityConditionDataTableRow> conditionDataList = new List<AbilityConditionDataTableRow>();
        
        private List<MethodInfo> actionMethods;
        private List<AbilityActionDataTableRow> actionDataList = new List<AbilityActionDataTableRow>();
        private Action<IAbilityContext> _onPreExecute;
        private Action<IAbilityContext> _onPostExecute;

        public AbilityEffect(string id, List<string> tags)
        {
            this.id = id;
            this.tags = tags.ToArray();
            orConditionMethods = new List<List<MethodInfo>>();
            actionMethods = new List<MethodInfo>();
        }
        
        public void Setup(AbilityAgent owner, object source)
        {
            this.owner = owner;
            this.source = source;
        }
        
        
        public void AddNewOrConditionGroup()
        {
            currentAndConditionGroup = new List<MethodInfo>();
            orConditionMethods.Add(currentAndConditionGroup);
        }

        public void BindAndCondition(MethodInfo condition,AbilityConditionDataTableRow data)
        {
            currentAndConditionGroup.Add(condition);
            conditionDataList.Add(data);
        }

        public void BindAction(MethodInfo action,AbilityActionDataTableRow data)
        {
            actionMethods.Add(action);
            actionDataList.Add(data);
        }


        public void InvokeAction<TAbilityContext>(ref TAbilityContext context) where TAbilityContext : IAbilityContext
        {
            for (int i = 0; i < actionMethods.Count; i++)
            {
               actionMethods[i]?.Invoke(null, new object[] { this, actionDataList[i], context });
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
                    andConditionResult &= (bool)andCondition.Invoke(null, new object[] { this, conditionDataList[i], context });
                }

                result |= andConditionResult;
            }

            return result;
        }


        public bool TryExecute<TAbilityContext>(ref TAbilityContext result) where TAbilityContext : IAbilityContext
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

        public void RegisterPreExecutionAction(Action<IAbilityContext> action)
        {
            _onPreExecute += action;
        }
        
        public void RegisterPostExecutionAction(Action<IAbilityContext> action)
        {
            _onPostExecute += action;
        }
    }
}