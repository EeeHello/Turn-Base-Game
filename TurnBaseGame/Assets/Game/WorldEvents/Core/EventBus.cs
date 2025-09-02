using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    /// <summary>
    /// Lightweight, type-safe pub/sub. No strings, no magic.
    /// Publish<T>(payload), Subscribe<T>(Action<T>).
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subs = new();

        /// <summary>
        /// Subscribe to events of type T. Returns an IDisposable you should Dispose (or store and call in OnDisable).
        /// </summary>
        public static IDisposable Subscribe<T> (Action<T> handler)
        {
            var t = typeof(T);
            if (!_subs.TryGetValue(t, out var list))
            {
                list = new List<Delegate>();
                _subs[t] = list;
            }
            list.Add(handler);
            return new Subscription<T>(handler);
        }

        /// <summary>
        /// Publish an event of type T to all subscribers.
        /// </summary>
        public static void Publish<T>(T evt)
        {
            var t = typeof(T);
            Debug.Log($"[EventBus] Publishing event of type: {t.Name}");

            if (_subs.TryGetValue(t, out var list))
            {
                Debug.Log($"[EventBus] Found {list.Count} subscribers for {t.Name}");

                // Copy to avoid modification during iteration
                var snapshot = list.ToArray();
                for (int i = 0; i < snapshot.Length; i++)
                {
                    try
                    {
                        ((Action<T>)snapshot[i])?.Invoke(evt);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
            else
            {
                Debug.Log($"[EventBus] No subscribers found for {t.Name}");
            }

#if UNITY_EDITOR
            if (EventBusDebug.Enabled)
                EventBusDebug.LogPublish(t, evt);
#endif
        }

        /// <summary>
        /// Clear all subscriptions (useful on playmode reload in editor-only tools).
        /// </summary>
        public static void Clear() => _subs.Clear();

        private sealed class Subscription<T> : IDisposable
        {
            private Action<T> _handler;
            public Subscription(Action<T> handler)
            {
                _handler = handler;
            }

            public void Dispose()
            {
                if (_handler == null)
                {
                    return;
                }

                var t = typeof(T);
                if (_subs.TryGetValue(t, out var list))
                {
                    list.Remove(_handler);
                }
                _handler = null;
            }
        }


    }

}
