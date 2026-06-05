using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    public static class CoroutineExtension
    {
        
        public static Task AsTask(this IEnumerator coroutine, MonoBehaviour owner)
        {       
            var tcs = new TaskCompletionSource<bool>();
            owner.StartCoroutine(Wrap(coroutine, tcs));
            return tcs.Task;
        }

        private static IEnumerator Wrap(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
        {
            yield return coroutine;
            tcs.SetResult(true);
        }
    }
}