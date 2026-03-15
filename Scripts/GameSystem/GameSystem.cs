using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    
    public class GameSystem : Singleton<GameSystem>
    {
        private static IGameSubSystem[] _subSystems;
        private static bool _isRunning = false;

        public void Awake()
        {
            _subSystems = GetComponentsInChildren<IGameSubSystem>(true);
            foreach (var subSystem in _subSystems)
            {
                subSystem.Setup(this);
            }
        }

        public void Start()
        {
            StartCoroutine(Run());
        }

        public T GetSubSystem<T>() where T : class
        {
            return Array.Find(_subSystems, s => s is T) as T;
        }
        
        public static IEnumerator Run()
        {
            foreach (var subSystem in _subSystems)
            {
                yield return subSystem.OnInitialize();
            }

            _isRunning = true;
            
            while (_isRunning)
            {
                foreach (var subSystem in _subSystems)
                {
                    yield return subSystem.OnUpdate();
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}