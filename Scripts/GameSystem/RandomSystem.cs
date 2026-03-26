using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace KCoreKit
{
    public static class RandomSystem
    {
        private static Random random;

        public static void SetSeed(int seed = 0)
        {
            random = new Random(seed);
        }
        
        public static T GetRandomElement<T>(this IList<T> array)
        {
            if (array == null || array.Count == 0)
            {
                return default;
            }
            return array.OrderBy(x => random.Next()).First();
        }
        public static List<T> GetRandomElements<T>(this IList<T> array, int number)
        {
            // 1. 방어 코드: 소스가 없거나 요청 개수가 0 이하일 때 빈 리스트 반환
            if (array == null || array.Count == 0 || number <= 0)
            {
                return new List<T>();
            }

            // 2. 요청 개수가 원본보다 많으면 원본 크기로 제한
            int count = Math.Min(number, array.Count);

            // 3. 효율적인 추출 (Fisher-Yates 셔플 응용 또는 LINQ 활용)
            // 소규모라면 OrderBy도 괜찮지만, 아래 방식이 가독성과 범용성이 좋습니다.
            return array.OrderBy(x => random.Next())
                .Take(count)
                .ToList();
        }
        public static float Range(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        
        public static int RangeInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public static Vector3 RangeVector3(Vector3 min, Vector3 max)
        {
            var x = (float)(random.NextDouble() * (max.x - min.x) + min.x);
            var y = (float)(random.NextDouble() * (max.y - min.y) + min.y);
            var z = (float)(random.NextDouble() * (max.z - min.z) + min.z);
            return new Vector3(x, y, z);
        }
    }
}