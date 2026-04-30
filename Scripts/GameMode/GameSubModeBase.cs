using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class GameSubModeBase : MonoBehaviour, IGameSubMode
    {
        protected GameMode gameMode;
        public void Setup(GameMode gameMode)
        {
            this.gameMode = gameMode;
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