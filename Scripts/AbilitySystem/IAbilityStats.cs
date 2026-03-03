using System.Collections.Generic;

namespace KCoreKit
{
    public interface IAbilityStats 
    {
        IEnumerable<AbilityStat> Get();
    }
}