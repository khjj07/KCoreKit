using System;
using UnityEditor;
using UnityEngine;

namespace KCoreKit
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T GetInstance()
        {
            if (_instance == null)
            {
                try
                {
                    _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e.StackTrace);
                    return null;
                }
            }
            return _instance;
        }
    }

}