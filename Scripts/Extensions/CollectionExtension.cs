
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public static class CollectionExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var randomIndex = Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}