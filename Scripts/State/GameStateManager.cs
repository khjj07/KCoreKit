using System;
using UnityEngine;

namespace KCoreKit
{
    public class GameStateManager<T> :MonoBehaviour where T : GameStateBase
    {
        private T _current;
        private T[] _states;

        public virtual void Awake()
        {
            _states = GetComponentsInChildren<T>(true);
            SetState(_states[0]);
        }
        public void SetState(T state)
        {
            _current?.OnExit();
            _current = state;
            _current?.OnEnter();
        }

        public void SetState(string name)
        {
           var state =  Array.Find(_states,x=>x.gameObject.name==name);
           SetState(state);
        }
    }
}