using System.Collections.Generic;

namespace KCoreKit
{
    public interface IAbilityStatSet 
    {
        IEnumerable<AbilityStat> Get();
    }
}