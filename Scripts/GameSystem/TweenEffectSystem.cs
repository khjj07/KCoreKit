using System.Collections.Generic;
using DG.Tweening;

namespace KCoreKit
{
    public class TweenEffectSystem : GameSubSystemBase
    {
        private Dictionary<string, List<Tween>> _tweenEffects = new();

        public void Register(string id, Tween tween)
        {
            if (!_tweenEffects.TryGetValue(id, out var list))
            {
                list = new List<Tween>();
                _tweenEffects[id] = list;
            }

            list.Add(tween);
        }

        public void Unregister(string id, Tween tween)
        {
            _tweenEffects[id].Remove(tween);
            if (_tweenEffects[id].Count == 0)
            {
                _tweenEffects.Remove(id);
            }
        }

        public void Play(string id)
        {
            foreach (var tween in _tweenEffects[id])
            {
                tween.Play();
            }
        }

        public void Pause(string id)
        {
            foreach (var tween in _tweenEffects[id])
            {
                tween.Pause();
            }
        }
    }
}