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
    public class InputManager : Singleton<InputManager>
    {
        private static PlayerInput _playerInput => GetInstance().GetComponent<PlayerInput>();
        

        public static void RegisterAction(Action<InputAction.CallbackContext> callback)
        {
            _playerInput.onActionTriggered += callback;
        }
        
        public static void RegisterAction(string action, PlayerActionType actionType,
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
        
        public static void UnregisterAction(string action, PlayerActionType actionType,
            Action<InputAction.CallbackContext> callback)
        {
            switch (actionType)
            {
                case PlayerActionType.Started:
                    _playerInput.actions[action].started -= callback;
                    break;
                case PlayerActionType.Performed:
                    _playerInput.actions[action].performed -= callback;
                    break;
                case PlayerActionType.Canceled:
                    _playerInput.actions[action].canceled -= callback;
                    break;
            }
        }
    }


}