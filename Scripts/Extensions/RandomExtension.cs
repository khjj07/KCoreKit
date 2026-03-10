using System;
using System.Collections.Generic;
using System.Linq;

namespace KCoreKit
{
    public static class RandomExtension
    {
        private static Random random;

        public static void SetRandomSeed(int seed = 0)
        {
            random = new Random(seed);
        }
        
        public static T GetRandomElement<T>(this IList<T> array)
        {
            return array.OrderBy(x => random.Next()).First();
        }
    }
    
    
}