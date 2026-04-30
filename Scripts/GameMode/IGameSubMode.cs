using System.Collections;

namespace KCoreKit
{
    public interface IGameSubMode
    {
        public abstract void Setup(GameMode gameMode);
        public abstract IEnumerator OnInitialize();
        public abstract IEnumerator OnUpdate();
    }
}