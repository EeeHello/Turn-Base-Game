using System;
using System.Collections.Generic;

namespace Game.WorldEvents.Core
{
    public static class WorldEventManager
    {
        private static readonly Dictionary<Type, List<Delegate>> listeners = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!listeners.ContainsKey(type))
                listeners[type] = new List<Delegate>();

            listeners[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (listeners.ContainsKey(type))
                listeners[type].Remove(callback);
        }

        public static void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (listeners.ContainsKey(type))
            {
                foreach (var callback in listeners[type])
                {
                    (callback as Action<T>)?.Invoke(evt);
                }
            }
        }
    }
}
