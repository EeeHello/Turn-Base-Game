using System;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    public static class WorldEventManager
    {
        // Wrap EventBus for backward compatibility
        public static void Subscribe<T>(Action<T> callback) => EventBus.Subscribe(callback);

        public static void Unsubscribe<T>(Action<T> callback)
        {
            // Note: EventBus uses IDisposable pattern instead of direct unsubscription
            // This method is kept for compatibility but may not work as expected
            Debug.LogWarning("[WorldEventManager] Use IDisposable from Subscribe instead of Unsubscribe for better memory management");
        }

        public static void Publish<T>(T evt) => EventBus.Publish(evt);
    }
}