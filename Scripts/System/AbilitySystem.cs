using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace KCoreKit
{
    public static class AbilitySystem
    {
        private static Dictionary<string,MethodInfo> actionMethods = new Dictionary<string,MethodInfo>();
        private static Dictionary<string,MethodInfo> conditionMethods = new Dictionary<string,MethodInfo>();
        private static List<AbilityDataTableRow> abilityDataList = new List<AbilityDataTableRow>(); 
        private static List<AbilityActionDataTableRow> abilityActionDataTableRow;
        private static List<AbilityConditionDataTableRow> abilityConditionDataTableRow;
    
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                            BindingFlags.DeclaredOnly;
        
        public static void Initialize()
        {
            abilityDataList = DataTableManager.FindAllRows<AbilityDataTableRow>();
            abilityConditionDataTableRow = DataTableManager.FindAllRows<AbilityConditionDataTableRow>();
            abilityActionDataTableRow = DataTableManager.FindAllRows<AbilityActionDataTableRow>();
        }

        public static void AddActionMethods(Type actionType)
        {
            var methodInfos = actionType.GetMethods(Flags).ToDictionary(x=>x.Name,x=>x);
            foreach (var info in methodInfos)
            {
                actionMethods[info.Key] = info.Value;
            }      
        }       
        
        public static void AddConditionMethods(Type conditionType)
        {
            var methodInfos = conditionType.GetMethods(Flags).ToDictionary(x=>x.Name,x=>x);
            foreach (var info in methodInfos)
            {
                conditionMethods[info.Key] = info.Value;
            }      
        }

        public static AbilityEffect CreateAbilityEffect(string id)
        {
            var data = abilityDataList.Find(x => x.id == id);
            var effect = new AbilityEffect(id,data.tags);
            //컨디션 바인딩
            foreach (var conditionId in data.abilityConditionIdList)
            {
                effect.AddNewOrConditionGroup();
                var andConditionList = conditionId.ParseStringList('&');
                foreach (var andConditionId in andConditionList)
                {
                    var condition = abilityConditionDataTableRow.Find(x => x.id == conditionId);
                    var conditionFunction = FindConditionMethod(condition.conditionFunctionName);
                    effect.BindAndCondition(conditionFunction,condition);
                }
            }

            //액션 바인딩
            foreach (var actionId in data.abilityActionIdList)
            {
                var action = abilityActionDataTableRow.Find(x => x.id == actionId);
                var actionFunction = FindActionMethod(action.actionFunctionName);
                effect.BindAction(actionFunction, action);
            }

            return effect;
        }

        private static MethodInfo FindActionMethod(string functionName)
        {
            return actionMethods[functionName];
        }

        private static MethodInfo FindConditionMethod(string functionName)
        {
            return conditionMethods[functionName];
        }

       
    }
}