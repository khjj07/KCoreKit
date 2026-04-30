using System;
using KCoreKit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KCoreKit
{
    public enum PlayerActionType
    {
        Started,
        Performed,
        Canceled
    }
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputMode : GameSubModeBase
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponentInChildren<PlayerInput>();
        }

        public void RegisterAction(Action<InputAction.CallbackContext> callback)
        {
            _playerInput.onActionTriggered += callback;
        }
        
        public void RegisterAction(string action, PlayerActionType actionType,
            Action<InputAction.CallbackContext> callback)
        {
            switch (actionType)
            {
                case PlayerActionType.Started:
                    _playerInput.actions[action].started += callback;
                    break;
                case PlayerActionType.Performed:
                    _playerInput.actions[action].performed += callback;
                    break;
                case PlayerActionType.Canceled:
                    _playerInput.actions[action].canceled += callback;
                    break;
            }
        }
    }


}