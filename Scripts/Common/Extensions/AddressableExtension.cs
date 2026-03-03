using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace KCoreKit
{
    public static class AddressableExtension
    {
        public static Task<AsyncOperationHandle<T>> LoadAsset<T>(string key, Action<T> onSuccess, Action onFail = null)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += (h) =>
            {
                if (h.Status == AsyncOperationStatus.Succeeded)
                {
                    onSuccess?.Invoke(h.Result);
                }
                else
                {
                    UnityEngine.Debug.LogError($"{key} 로딩 실패 : {h.OperationException.Message}");
                    onFail?.Invoke();
                }
            };
            return Task.FromResult(handle);
        }

        public static Task<AsyncOperationHandle<IList<T>>> LoadAssets<T>(string key, Action<T> onEachSuccess,
            Action<IList<T>> onSuccess = null, Action onFail = null)
        {
            var handle = Addressables.LoadAssetsAsync<T>(key, onEachSuccess);
            handle.Completed += (h) =>
            {
                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    UnityEngine.Debug.LogError($"{key} 로딩 실패 : {h.OperationException.Message}");
                    onFail?.Invoke();
                }
                else
                {
                    onSuccess?.Invoke(h.Result);
                }
            };
            return Task.FromResult(handle);
        }
    }
}