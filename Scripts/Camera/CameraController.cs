using System;
using Unity.Cinemachine;
using UnityEngine;

namespace KCoreKit
{
    [Serializable]
    public class CameraControllerOption
    {
        public CursorLockMode cursorLockMode;
        public bool cursorVisible;
    }
    public class CameraController : MonoBehaviour
    {
        protected CinemachineCamera camera;
        
        [SerializeField]
        private CameraControllerOption  option;

        public virtual void Awake()
        {
            camera =  GetComponent<CinemachineCamera>();
        }
        public virtual void OnActive()
        {
            Cursor.lockState = option.cursorLockMode;
            Cursor.visible = option.cursorVisible;
        }

        public virtual void OnInActive()
        {
            
        }

        public void SetAnchor(Transform anchor)
        {
            
        }

        public virtual void Reposition(Vector3 position, Quaternion rotation)
        {
            camera.ForceCameraPosition(position,rotation);
        }

        public void SetFollow(Transform follow)
        {
            camera.Follow = follow;
        }
    }
}