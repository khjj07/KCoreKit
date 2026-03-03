using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KCoreKit
{
    public class CameraControllerManager : Singleton<CameraControllerManager>
    {
        private List<CameraController> _cameraControllers;
        private CameraController _currentController;
        private UnityEngine.Camera _camera;

        public void Awake()
        {
            _camera = UnityEngine.Camera.main;
            _cameraControllers = GetComponentsInChildren<CameraController>().ToList();
        }

        public static void SetController(CameraController camera)
        {
            var instance = GetInstance();
            if (instance._currentController)
            {
                instance._currentController.OnInActive();
                instance._currentController.gameObject.SetActive(false);
            }

            instance._currentController = camera;
            instance._currentController.OnActive();
            instance._currentController.gameObject.SetActive(true);
        }

        public static void RepositionCamera(Vector3 position, Quaternion rotation)
        {
            var instance = GetInstance();
            foreach (var controller in instance._cameraControllers)
            {
                controller.Reposition(position, rotation);
            }
        }

        public static UnityEngine.Camera GetCamera()
        {
            return GetInstance()._camera;
        }
    }
}