using System;
using System.Collections;
using UnityEngine;

namespace KCoreKit
{
    public interface IRequest
    {
        public IEnumerator Wait();
        public void Proceed();
    }

    public class RequestBase : IRequest
    {
        private bool _isWaiting;
        public Action<RequestBase> OnResponseCallback;
        private IEnumerator _waitEnumerator;

        public RequestBase()
        {
            _waitEnumerator = new WaitUntil(()=>!IsWaiting());
        }
        public IEnumerator Wait()
        {
            _isWaiting = true;
            yield return _waitEnumerator;
            OnResponse();
        }

        public void Proceed()
        {
            _isWaiting = false;
        }
     
        public void OnResponse()
        {
           OnResponseCallback?.Invoke(this);
        }
        
        public bool IsWaiting()
        {
            return _isWaiting;
        }
    }
}