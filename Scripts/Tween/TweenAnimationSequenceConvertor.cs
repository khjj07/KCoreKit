using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

namespace KCoreKit
{
    public class TweenAnimationSequenceConvertor : MonoBehaviour
    {
        private Sequence _sequence;

        // Awake 대신 Start를 사용하여 모든 컴포넌트의 초기화 완료를 보장
        public void Start()
        {
            _sequence = DOTween.Sequence().SetAutoKill(false).Pause();

            var animationComponents = GetComponents<ABSAnimationComponent>();
            foreach (var component in animationComponents)
            {
                // tween이 정상적으로 생성되었는지 확인 후 추가
                if (component.tween != null)
                {
                    _sequence.Join(component.tween);
                }
            }
        }

        public Sequence GetSequence()
        {
            return _sequence;
        }

        [Button]
        public void TestPlay()
        {
            Play();
        }

        public Sequence Play()
        {
            if (_sequence != null)
            {
                _sequence.Restart();
            }
            return _sequence;
        }
    }
}