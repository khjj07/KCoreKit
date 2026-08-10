using System;
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
        private static Mouse _mouse;
        private static Camera _camera;

        public void Start()
        {
            _mouse = Mouse.current;
            _camera = Camera.main;
        }

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

        public static Vector3 GetWorldMousePosition(float z = 0)
        {
            var mousePosition = _mouse.position.ReadValue();
            var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = z;
            return worldPosition;
        }
    }
}