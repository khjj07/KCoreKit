using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KCoreKit
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        private IEnumerator LoadSceneAsync(string sceneName, Action onSceneLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);
            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
            onSceneLoaded?.Invoke();
        }

        public static void ChangeScene(string sceneName, Action onSceneLoaded = null)
        {
            var instance = GetInstance();
            instance.StartCoroutine(instance.LoadSceneAsync(sceneName, onSceneLoaded));
        }
    }
}