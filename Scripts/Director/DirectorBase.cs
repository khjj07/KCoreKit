using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class DirectorBase : MonoBehaviour, IDirector
    {
        

        public virtual IEnumerator OnInitialize()
        {
            yield return null;
        }
    }
}