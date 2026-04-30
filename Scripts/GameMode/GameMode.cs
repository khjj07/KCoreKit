using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    
    public class GameMode : Singleton<GameMode>
    {
        private static bool isInitialized;
        private static IGameSubMode[] _subModes;
        private static bool _isRunning = false;

        public void Awake()
        {
            _subModes = GetComponentsInChildren<IGameSubMode>(true);
            foreach (var subMode in _subModes)
            {
                subMode.Setup(this);
            }
        }

        public void Start()
        {
            StartCoroutine(Run());
        }

        public static T GetSubMode<T>() where T : class
        {
            return Array.Find(_subModes, s => s is T) as T;
        }

        public static IEnumerator WaitUntilInitialized()
        {
            yield return new WaitUntil(() => isInitialized);
        }
        
        public static IEnumerator Run()
        {
            foreach (var subSystem in _subModes)
            {
                yield return subSystem.OnInitialize();
            }

            isInitialized = true;
            _isRunning = true;
            
            while (_isRunning)
            {
                foreach (var subSystem in _subModes)
                {
                    yield return subSystem.OnUpdate();
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}