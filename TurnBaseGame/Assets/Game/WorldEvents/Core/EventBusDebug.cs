#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Game.WorldEvents.Core
{
    public static class EventBusDebug
    {
        public static bool Enabled = true;
        public static void LogPublish(Type t, object payload)
        {
            Debug.Log($"[EventBus] {t.Name} -> {JsonSafe(payload)}");
        }
        private static string JsonSafe(object o)
        {
            if (o == null) return "null";
            try { return JsonUtility.ToJson(o); } catch { return o.ToString(); }
        }
    }
}
#endif