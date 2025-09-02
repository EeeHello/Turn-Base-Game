using System;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    public abstract class EventListenerBehaviour<T> : MonoBehaviour
    {
        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription = EventBus.Subscribe<T>(OnEvent, this);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void OnEvent(T evt);
    }
}