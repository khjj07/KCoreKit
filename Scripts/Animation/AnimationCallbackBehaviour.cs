using System;
using UnityEngine;

namespace KCoreKit
{
    public class AnimationCallbackBehaviour : StateMachineBehaviour
    {
        public Action<AnimatorStateInfo> callback;
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                callback?.Invoke(stateInfo);
            }
        }
    
    }
}
