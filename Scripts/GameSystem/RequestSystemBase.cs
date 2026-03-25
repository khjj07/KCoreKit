using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KCoreKit
{
    public abstract class RequestSystemBase : GameSubSystemBase
    {
        private Queue<IRequest> _requestQueue = new Queue<IRequest>();
        [SerializeField] private int maxRequestPerFrame = 10;

        public void SendRequest(IRequest request)
        {
            _requestQueue.Enqueue(request);

            StartCoroutine(request.Wait());
        }

        protected abstract void Respond(IRequest request);

        public override IEnumerator OnUpdate()
        {
            yield return RespondRequestRoutine();
        }

        private IEnumerator RespondRequestRoutine()
        {
            for (int i = 0; i < maxRequestPerFrame; i++)
            {
                if (_requestQueue.Count == 0)
                {
                    break;
                }

                var request = _requestQueue.Dequeue();
                Respond(request);
                request.Proceed();
            }

            yield return null;
        }
    }
}