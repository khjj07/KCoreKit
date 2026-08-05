using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace KCoreKit.Scripts.Common
{
    public class PrefabPool<T> where T : MonoBehaviour
    {
        private ObjectPool<T> _pool;
        private Transform _parent;
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale = Vector3.one;
        private T _prefab;
        
        public Action<T> onGetAction;
        public Action<T> onReleaseAction;

        public PrefabPool(T prefab, Transform parent = null, int capacity = 10)
        {
            _prefab = prefab;
            _parent = parent;
            _pool = new ObjectPool<T>(CreateInstance, GetInstance, ReleaseInstance, DestroyInstance, true, capacity);
        }

        public void SetScale(Vector3 scale)
        {
            _scale = scale;
        }

        public void SetRotation(Quaternion rotation)
        {
            _rotation = rotation;
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        private T CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _parent, true);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void DestroyInstance(T instance)
        {
            Object.Destroy(instance.gameObject);
        }

        private void ReleaseInstance(T instance)
        {
            instance.gameObject.SetActive(false);
            onReleaseAction?.Invoke(instance);
        }

        private void GetInstance(T instance)
        {
            instance.gameObject.SetActive(true);
            instance.transform.position = _position;
            instance.transform.rotation = _rotation;
            instance.transform.localScale = _scale;
            onGetAction?.Invoke(instance);
        }

        public T Get()
        {
            return _pool.Get();
        }

        public void Release(T instance)
        {
            _pool.Release(instance);
        }
    }
}