using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class DirectorBase : MonoBehaviour, IDirector
    {
        protected DirectorFacade DirectorFacade;
        public void Setup(DirectorFacade directorFacade)
        {
            this.DirectorFacade = directorFacade;
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