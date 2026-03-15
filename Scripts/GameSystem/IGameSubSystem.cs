using System.Collections;

namespace KCoreKit
{
    public interface IGameSubSystem
    {
        public abstract void Setup(GameSystem gameSystem);
        public abstract IEnumerator OnInitialize();
        public abstract IEnumerator OnUpdate();
    }
}