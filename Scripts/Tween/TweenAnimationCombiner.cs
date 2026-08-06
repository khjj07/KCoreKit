using System;
using System.Collections;
using DG.DOTweenEditor;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace KCoreKit
{
    public enum TweenCombineMode
    {
        Join,
        Append
    }

    public class TweenAnimationCombiner : MonoBehaviour
    {
        public TweenCombineMode mode;
        [SerializeField] private bool isRoot;


        public IEnumerator Play()
        {
            yield return PlayRecursiveRoutine();
        }

        private IEnumerator PlayRoutine(Action callback = null)
        {
            var components = GetComponents<ABSAnimationComponent>();
            var count = 0;
            foreach (var component in components)
            {
                switch (mode)
                {
                    case TweenCombineMode.Join:
                        count++;
                        component.tween.Play().OnComplete(() => count--);
                        break;
                    case TweenCombineMode.Append:
                        yield return component.tween.Play().WaitForCompletion();
                        break;
                }
            }

            yield return new WaitUntil(() => count == 0);
            callback?.Invoke();
        }

        private IEnumerator PlayRecursiveRoutine()
        {
            if (isRoot)
            {
                int count = 0;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform directChild = transform.GetChild(i);
                    var combiner = directChild.GetComponent<TweenAnimationCombiner>();

                    // 자식에게 Combiner가 존재할 경우에만 처리
                    if (combiner != null)
                    {
                        // 자식의 mode 설정에 따라 부모 시퀀스에 결합
                        switch (combiner.mode)
                        {
                            case TweenCombineMode.Join:
                                count++;
                                StartCoroutine(combiner.PlayRoutine(() => count--));
                                break;
                            case TweenCombineMode.Append:
                                yield return combiner.PlayRoutine();
                                break;
                        }
                    }
                }

                yield return new WaitUntil(() => count == 0);
            }
        }
    }
}