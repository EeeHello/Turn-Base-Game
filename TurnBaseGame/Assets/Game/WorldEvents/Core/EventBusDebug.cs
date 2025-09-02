#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    public static class EventBusDebug
    {
        public static bool Enabled = true;
        public static bool LogSubscriptions = true;
        public static bool ShowSubscriberNames = true;

        private static readonly Dictionary<Type, List<SubscriberInfo>> _subscriberInfo = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetOnPlayMode()
        {
            Enabled = true;
            LogSubscriptions = true;
            ShowSubscriberNames = true;
            _subscriberInfo.Clear();
            EventBus.Clear();
        }

        public static void LogPublish(Type eventType, object payload)
        {
            if (!Enabled) return;

            var subscriberCount = _subscriberInfo.ContainsKey(eventType) ? _subscriberInfo[eventType].Count : 0;
            var message = $"[EventBus] Publishing {eventType.Name} to {subscriberCount} subscribers -> {JsonSafe(payload)}";

            if (subscriberCount > 0 && ShowSubscriberNames)
            {
                message += $"\nSubscribers: {GetSubscriberNames(eventType)}";
            }

            Debug.Log(message);
        }

        public static void LogSubscribe(Type eventType, Delegate handler, object subscriber = null)
        {
            if (!Enabled || !LogSubscriptions) return;

            var subscriberName = GetSubscriberName(handler, subscriber);
            AddSubscriberInfo(eventType, handler, subscriberName);

            Debug.Log($"[EventBus] Subscribed to {eventType.Name} -> {subscriberName}");
            Debug.Log($"[EventBus] Total subscribers for {eventType.Name}: {GetSubscriberCount(eventType)}");
        }

        public static void LogUnsubscribe(Type eventType, Delegate handler, object subscriber = null)
        {
            if (!Enabled || !LogSubscriptions) return;

            var subscriberName = GetSubscriberName(handler, subscriber);
            RemoveSubscriberInfo(eventType, handler);

            Debug.Log($"[EventBus] Unsubscribed from {eventType.Name} -> {subscriberName}");
            Debug.Log($"[EventBus] Remaining subscribers for {eventType.Name}: {GetSubscriberCount(eventType)}");
        }

        public static string GetSubscriberStats()
        {
            if (_subscriberInfo.Count == 0) return "No active subscriptions";

            var sb = new StringBuilder();
            sb.AppendLine("[EventBus] Subscription Statistics:");

            foreach (var kvp in _subscriberInfo.OrderByDescending(x => x.Value.Count))
            {
                sb.AppendLine($"  {kvp.Key.Name}: {kvp.Value.Count} subscribers");
                if (ShowSubscriberNames)
                {
                    foreach (var subscriber in kvp.Value)
                    {
                        sb.AppendLine($"    - {subscriber.Name} ({subscriber.Timestamp:HH:mm:ss})");
                    }
                }
            }

            return sb.ToString();
        }

        public static void PrintSubscriberStats()
        {
            if (Enabled) Debug.Log(GetSubscriberStats());
        }

        private static void AddSubscriberInfo(Type eventType, Delegate handler, string subscriberName)
        {
            if (!_subscriberInfo.ContainsKey(eventType))
                _subscriberInfo[eventType] = new List<SubscriberInfo>();

            var existing = _subscriberInfo[eventType].FirstOrDefault(s => s.Handler == handler);
            if (existing == null)
            {
                _subscriberInfo[eventType].Add(new SubscriberInfo
                {
                    Handler = handler,
                    Name = subscriberName,
                    Timestamp = DateTime.Now
                });
            }
        }

        private static void RemoveSubscriberInfo(Type eventType, Delegate handler)
        {
            if (_subscriberInfo.ContainsKey(eventType))
            {
                _subscriberInfo[eventType].RemoveAll(s => s.Handler == handler);
                if (_subscriberInfo[eventType].Count == 0)
                    _subscriberInfo.Remove(eventType);
            }
        }

        private static int GetSubscriberCount(Type eventType)
        {
            return _subscriberInfo.ContainsKey(eventType) ? _subscriberInfo[eventType].Count : 0;
        }

        private static string GetSubscriberNames(Type eventType)
        {
            if (!_subscriberInfo.ContainsKey(eventType)) return "None";

            return string.Join(", ", _subscriberInfo[eventType].Select(s => s.Name));
        }

        private static string GetSubscriberName(Delegate handler, object subscriber)
        {
            if (subscriber != null)
            {
                if (subscriber is UnityEngine.Object unityObj && unityObj != null)
                    return $"{unityObj.GetType().Name} ({unityObj.name})";

                return subscriber.GetType().Name;
            }

            if (handler != null)
            {
                return $"{handler.Method.DeclaringType?.Name}.{handler.Method.Name}";
            }

            return "Unknown Subscriber";
        }

        private static string JsonSafe(object o)
        {
            if (o == null) return "null";
            try { return JsonUtility.ToJson(o); }
            catch { return o.ToString(); }
        }

        private class SubscriberInfo
        {
            public Delegate Handler { get; set; }
            public string Name { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
#endif