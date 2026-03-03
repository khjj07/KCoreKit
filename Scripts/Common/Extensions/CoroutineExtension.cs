using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace KCoreKit
{
    public static class CoroutineExtension
    {
        public static IEnumerator WaitForTween(Tweener tween)
        {
            bool done = false;
            tween.OnComplete(() => { done = true; });
            tween.Play();
            while (!done)
            {
                yield return null;
            }

            yield return null;
        }
        
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