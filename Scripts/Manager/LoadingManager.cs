using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KCoreKit
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        private static AsyncOperation _currentOperation;
        private static bool _blockLoading;

        private IEnumerator LoadSceneAsync(string sceneName,bool useAutoSceneActivation, Action onSceneLoaded)
        {
            _currentOperation = SceneManager.LoadSceneAsync(sceneName);
            
            if (_currentOperation != null)
            {
                _currentOperation.allowSceneActivation = false;
                yield return new WaitUntil(() => _currentOperation.progress >= 0.9f && !_blockLoading);
                if (useAutoSceneActivation)
                {
                    _currentOperation.allowSceneActivation = true;
                }

                yield return new WaitUntil(() => _currentOperation.isDone);
            }

            onSceneLoaded?.Invoke();
        }

        public static void LoadScene(string sceneName, float minDuration = 0, bool useAutoSceneActivation = false, Action onSceneLoaded = null)
        {
            var instance = GetInstance();
            _blockLoading = true;
            instance.StartCoroutine(WaitForMinLoadingDuration(minDuration));
            instance.StartCoroutine(instance.LoadSceneAsync(sceneName, useAutoSceneActivation, onSceneLoaded));
        }

        public static void ActivateScene()
        {
            if (_currentOperation != null)
            {
                _currentOperation.allowSceneActivation = true;
            }
        }

        private static IEnumerator WaitForMinLoadingDuration(float minDuration)
        {
            yield return new WaitForSeconds(minDuration);
            _blockLoading = false;
        }
    }
}