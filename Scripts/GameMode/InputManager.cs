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
        private static PlayerInput PlayerInput => GetInstance().GetComponent<PlayerInput>();
        

        public static void RegisterAction(Action<InputAction.CallbackContext> callback)
        {
            PlayerInput.onActionTriggered += callback;
        }
        
        public static void RegisterAction(string action, PlayerActionType actionType,
            Action<InputAction.CallbackContext> callback)
        {
            switch (actionType)
            {
                case PlayerActionType.Started:
                    PlayerInput.actions[action].started += callback;
                    break;
                case PlayerActionType.Performed:
                    PlayerInput.actions[action].performed += callback;
                    break;
                case PlayerActionType.Canceled:
                    PlayerInput.actions[action].canceled += callback;
                    break;
            }
        }
    }


}