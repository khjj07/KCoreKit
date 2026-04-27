using System.Collections.Generic;

namespace KCoreKit
{
    public class AbilityConditionDataTableRow : DataTableRowBase
    {
        public string conditionFunctionName;
        public List<string> properties;

        public Dictionary<string, string> GetProperties()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var tmp = property.ParseStringList(':');
                results.Add(tmp[0], tmp[1]);
            }
            return results;
        }
    }
}