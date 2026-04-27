using System.Collections.Generic;
using KCoreKit;

namespace KCoreKit
{
    public class AbilityActionDataTableRow : DataTableRowBase
    {
        public string actionFunctionName;
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