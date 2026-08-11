using System;
using System.Collections;
using System.Collections.Generic;
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

    public class TweenAnimationPlayer : MonoBehaviour
    {
       
        public TweenCombineMode mode;
        public bool forcePlay;
        
        public IEnumerator Play(float delay = 0,Action action = null)
        {
            if (forcePlay)
            {
                var components = GetComponentsInChildren<ABSAnimationComponent>();
                foreach (var component in components)
                {
                    component.DORewind();
                }
            }

            yield return new WaitForSeconds(delay);
            yield return PlayRecursiveRoutine(action);
        }

        private IEnumerator PlayRecursiveRoutine(Action callback = null)
        {
            var selfComponents = GetComponents<ABSAnimationComponent>();

            if (mode == TweenCombineMode.Append)
            {
                // 1. Append 모드: 자신의 트윈을 순차적으로 완료될 때까지 대기
                foreach (var comp in selfComponents)
                {
                    if (comp != null && comp.tween != null)
                    {
                        comp.tween.Rewind();
                        yield return comp.tween.Play().WaitForCompletion();
                    }
                }

                // 2. 자식들 순차 처리
                yield return PlayChildrenAppendRoutine();
            }
            else // Join 모드
            {
                List<Tween> activeTweens = new List<Tween>();
                List<Coroutine> childCoroutines = new List<Coroutine>();

                // 1. 자신의 트윈들을 동시에 실행 시작
                foreach (var comp in selfComponents)
                {
                    if (comp != null && comp.tween != null)
                    {
                        comp.tween.Rewind();
                        activeTweens.Add(comp.tween.Play());
                    }
                }

                // 2. 자식들도 동시에 실행 시작 (Join)
                for (int i = 0; i < transform.childCount; i++)
                {
                    var childTransform = transform.GetChild(i);
                    var childCombiner = childTransform.GetComponent<TweenAnimationPlayer>();

                    if (childCombiner != null)
                    {
                        childCoroutines.Add(StartCoroutine(childCombiner.PlayRecursiveRoutine()));
                    }
                    else
                    {
                        // Combiner가 없고 컴포넌트만 있는 자식 처리
                        var anims = childTransform.GetComponents<ABSAnimationComponent>();
                        foreach (var anim in anims)
                        {
                            if (anim != null && anim.tween != null)
                            {
                                anim.tween.Rewind();
                                activeTweens.Add(anim.tween.Play());
                            }
                        }
                    }
                }

                // 3. 자신의 모든 트윈이 끝날 때까지 대기
                foreach (var tween in activeTweens)
                {
                    if (tween != null && tween.IsActive())
                    {
                        yield return tween.WaitForCompletion();
                    }
                }

                // 4. 자식 코루틴들이 모두 끝날 때까지 대기
                foreach (var co in childCoroutines)
                {
                    yield return co;
                }
            }

            // 모든 재생이 끝난 후 콜백 호출 보장
            callback?.Invoke();
        }

        private IEnumerator PlayChildrenAppendRoutine()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var childTransform = transform.GetChild(i);
                var childCombiner = childTransform.GetComponent<TweenAnimationPlayer>();

                if (childCombiner != null)
                {
                    yield return childCombiner.PlayRecursiveRoutine();
                }
                else
                {
                    var anims = childTransform.GetComponents<ABSAnimationComponent>();
                    foreach (var anim in anims)
                    {
                        if (anim != null && anim.tween != null)
                        {
                            anim.tween.Rewind();
                            yield return anim.tween.Play().WaitForCompletion();
                        }
                    }
                }
            }
        }
    }
}