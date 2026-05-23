using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace KCoreKit
{
    public static class AbilityManager
    {
        private static MethodInfo[] actionMethods;
        private static MethodInfo[] conditionMethods;
        private static List<AbilityDataTableRow> abilityDataList = new List<AbilityDataTableRow>(); 
        private static List<AbilityActionDataTableRow> abilityActionDataTableRow;
        private static List<AbilityConditionDataTableRow> abilityConditionDataTableRow;
      
        public static void Setup<TAction, TCondition>()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                 BindingFlags.DeclaredOnly;
            Type actionType = typeof(TAction);
            Type conditionType = typeof(TCondition);
            actionMethods = actionType.GetMethods(flags);
            conditionMethods = conditionType.GetMethods(flags);
            abilityDataList = DataTableManager.FindAllRows<AbilityDataTableRow>();
            abilityConditionDataTableRow = DataTableManager.FindAllRows<AbilityConditionDataTableRow>();
            abilityActionDataTableRow = DataTableManager.FindAllRows<AbilityActionDataTableRow>();
        }

        public static AbilityEffect CreateAbilityEffect(string id, IAbilityProvider provider)
        {
            var data = abilityDataList.Find(x => x.id == id);
            var effect = new AbilityEffect(id, provider,data.tags);
            //컨디션 바인딩
            foreach (var conditionId in data.abilityConditionIdList)
            {
                effect.AddNewOrConditionGroup();
                var andConditionList = conditionId.ParseStringList('&');
                foreach (var andConditionId in andConditionList)
                {
                    var condition = abilityConditionDataTableRow.Find(x => x.id == conditionId);
                    var conditionFunction = FindConditionMethod(condition.conditionFunctionName);
                    effect.BindAndCondition(conditionFunction,condition.GetProperties());
                }
            }

            //액션 바인딩
            foreach (var actionId in data.abilityActionIdList)
            {
                var action = abilityActionDataTableRow.Find(x => x.id == actionId);
                var actionFunction = FindActionMethod(action.actionFunctionName);
                effect.BindAction(actionFunction, action.GetProperties());
            }

            return effect;
        }

        private static MethodInfo FindActionMethod(string condition)
        {
            return Array.Find(actionMethods, x => x.Name == condition);
        }

        private static MethodInfo FindConditionMethod(string action)
        {
            return Array.Find(conditionMethods, x => action == x.Name);
        }

       
    }
}