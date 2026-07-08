using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    
    public class GameMode : Singleton<GameMode>
    {
        private bool isInitialized;
        private IGameSubMode[] _subModes;
        private bool _isRunning = false;

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
            return Array.Find(GetInstance()._subModes, s => s is T) as T;
        }

        public static IEnumerator WaitUntilInitialized()
        {
            yield return new WaitUntil(() => GetInstance().isInitialized);
        }
        
        public static IEnumerator Run()
        {
            var instance = GetInstance();
            foreach (var subMode in instance._subModes)
            {
                yield return subMode.OnInitialize();
            }
            instance.isInitialized = true;
            instance._isRunning = true;
            
            while (instance._isRunning)
            {
                foreach (var subMode in instance._subModes)
                {
                    yield return subMode.OnUpdate();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void OnDestroy()
        {
            isInitialized = false;
            _isRunning = false;
        }
    }
}