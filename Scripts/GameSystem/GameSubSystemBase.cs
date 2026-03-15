using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class GameSubSystemBase : MonoBehaviour, IGameSubSystem
    {
        protected GameSystem GameSystem;
        public void Setup(GameSystem gameSystem)
        {
            GameSystem = gameSystem;
        }

        public virtual IEnumerator OnInitialize()
        {
            yield return null;
        }

        public virtual IEnumerator OnUpdate()
        {
            yield return null;
        }
    }
}