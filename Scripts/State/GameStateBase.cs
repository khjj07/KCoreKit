using UnityEngine;

namespace KCoreKit
{
    public abstract class GameStateBase : MonoBehaviour
    {
        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
        }
        public virtual void OnExit()
        {
            gameObject.SetActive(false);
        }
    }
}