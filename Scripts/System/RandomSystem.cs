using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace KCoreKit
{
    public static class RandomSystem
    {
        private static int _counter;
        private static int _seed;
        private static Random _random;

        public static void SetSeed(int seed = 0)
        {
            _counter = 0;
            _seed = seed;
            _random = new Random(_seed);
        }
        
        public static T GetRandomElement<T>(this IList<T> array)
        {
            if (array == null || array.Count == 0)
            {
                return default;
            }
            return array.OrderBy(x => Next()).First();
        }
        public static T GetRandomElementWithWeight<T>(this IList<T> array, Func<T,float> weightGetter)
        {
            if (array == null || array.Count == 0)
            {
                return default;
            }

            var sum = array.Sum(weightGetter.Invoke);
            var randomVal = Range(0,sum);
            float accum = 0;
            foreach (var x in array)
            {
                accum += weightGetter.Invoke(x);
                if (randomVal > accum)
                {
                    return x;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
        public static List<T> GetRandomElements<T>(this IList<T> array, int number)
        {
            if (array == null || array.Count == 0 || number <= 0)
            {
                return new List<T>();
            }
            
            
            return array.OrderBy(x => Next())
                .Take(number)
                .ToList();
        }
        
        public static float Range(float min, float max)
        {
            return (float)NextDouble() * (max - min) + min;
        }
        
        public static int RangeInt(int min, int max)
        {
            return Next(min, max);
        }

        public static Vector3 RangeVector3(Vector3 min, Vector3 max)
        {
            var x = (float)(NextDouble() * (max.x - min.x) + min.x);
            var y = (float)(NextDouble() * (max.y - min.y) + min.y);
            var z = (float)(NextDouble() * (max.z - min.z) + min.z);
            return new Vector3(x, y, z);
        }

        public static double NextDouble()
        {
            _counter++;
            return _random.NextDouble();
        }

        public static float Next()
        {
            _counter++;
            return _random.Next();
        }

        public static int Next(int min, int max)
        {
            _counter++;
            return _random.Next(min, max);
        }
        
        public static int GetSeed()
        {
            return _seed;
        }

        public static int GetCounter()
        {
            return _counter;
        }
    }
}