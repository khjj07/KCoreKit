using System.Collections;

namespace KCoreKit
{
    public interface IDirector
    {
        public abstract void Setup(DirectorFacade directorFacade);
        public abstract IEnumerator OnInitialize();
        
        public abstract IEnumerator OnUpdate();
    }
}