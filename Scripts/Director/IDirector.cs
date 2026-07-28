using System.Collections;

namespace KCoreKit
{
    public interface IDirector
    {
        public abstract IEnumerator OnInitialize();
        
        public abstract IEnumerator OnUpdate();
    }
}