using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subs = new();
        private static readonly object _lock = new object();

        public static IDisposable Subscribe<T>(Action<T> handler)
        {
            return Subscribe<T>(handler, null);
        }

        public static IDisposable Subscribe<T>(Action<T> handler, object subscriber)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var t = typeof(T);
                if (!_subs.TryGetValue(t, out var list))
                {
                    list = new List<Delegate>();
                    _subs[t] = list;
                }
                list.Add(handler);

#if UNITY_EDITOR
                if (EventBusDebug.Enabled)
                    EventBusDebug.LogSubscribe(t, handler, subscriber);
#endif

                return new Subscription<T>(handler, subscriber);
            }
        }

        public static void Publish<T>(T evt)
        {
            if (evt == null)
            {
                Debug.LogWarning("[EventBus] Attempted to publish null event");
                return;
            }

            var t = typeof(T);

#if UNITY_EDITOR
            if (EventBusDebug.Enabled)
                EventBusDebug.LogPublish(t, evt);
#endif

            Delegate[] snapshot;
            lock (_lock)
            {
                if (!_subs.TryGetValue(t, out var list) || list.Count == 0)
                {
                    Debug.Log($"[EventBus] No subscribers found for {t.Name}");
                    return;
                }
                snapshot = list.ToArray();
            }

            Debug.Log($"[EventBus] Publishing {t.Name} to {snapshot.Length} subscribers");

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

        public static void Clear()
        {
            lock (_lock)
            {
                _subs.Clear();
#if UNITY_EDITOR
                if (EventBusDebug.Enabled)
                    Debug.Log("[EventBus] Cleared all subscriptions");
#endif
            }
        }

        public static int GetSubscriberCount<T>()
        {
            lock (_lock)
            {
                var t = typeof(T);
                return _subs.TryGetValue(t, out var list) ? list.Count : 0;
            }
        }

        private sealed class Subscription<T> : IDisposable
        {
            private Action<T> _handler;
            private object _subscriber;

            public Subscription(Action<T> handler, object subscriber)
            {
                _handler = handler;
                _subscriber = subscriber;
            }

            public void Dispose()
            {
                lock (_lock)
                {
                    if (_handler == null) return;

                    var t = typeof(T);
                    if (_subs.TryGetValue(t, out var list))
                    {
                        list.Remove(_handler);
                        if (list.Count == 0) _subs.Remove(t);

#if UNITY_EDITOR
                        if (EventBusDebug.Enabled)
                            EventBusDebug.LogUnsubscribe(t, _handler, _subscriber);
#endif
                    }
                    _handler = null;
                    _subscriber = null;
                }
            }
        }
    }
}