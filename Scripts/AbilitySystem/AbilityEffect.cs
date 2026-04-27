using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem;

namespace KCoreKit
{

    public class AbilityEffect
    {
        public string id;
        public AbilityAgent owner;
        public string[] tag;
        private bool _isActive;
        
        private List<List<MethodInfo>> orConditionMethods;
        private List<MethodInfo> currentAndConditionGroup;
        private List<Dictionary<string, string>> conditionProperties;
        
        private List<MethodInfo> actionMethods;
        private List<Dictionary<string, string>> actionProperties;

        public AbilityEffect(string id)
        {
            this.id = id;
        }
        
        public void Setup(AbilityAgent owner)
        {
            this.owner = owner;
            orConditionMethods = new List<List<MethodInfo>>();
            conditionProperties = new List<Dictionary<string, string>>();
            actionMethods = new List<MethodInfo>();
            actionProperties = new List<Dictionary<string, string>>();
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


        public IEnumerator InvokeAction<TArgumentData>(TArgumentData argumentData)
        {
            for (int i = 0; i < actionMethods.Count; i++)
            {
                yield return (IEnumerator)actionMethods[i].Invoke(null,
                    new object[] { this, actionProperties[i], argumentData });
            }
        }


        public bool EvaluateCondition<TArgumentData>(TArgumentData argumentData)
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


        public IEnumerator TryExecute<TArgument>(TArgument argumentData)
        {
            if (EvaluateCondition(argumentData))
            {
                yield return InvokeAction(argumentData);
            }
        }
    }
}