using System;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    /// <summary>
    /// Base class that auto-subscribes/unsubscribes to EventBus events of type T.
    /// </summary>
    public abstract class EventListenerBehaviour<T> : MonoBehaviour
    {
        private IDisposable _sub;
        protected virtual void OnEnable() => _sub = EventBus.Subscribe<T>(OnEvent);
        protected virtual void OnDisable() { _sub?.Dispose(); _sub = null; }
        protected abstract void OnEvent(T evt);
    }
}