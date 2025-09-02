using System;

namespace Game.WorldEvents.Core
{
    public static class EventBusExtensions
    {
        public static IDisposable SubscribeOnce<T>(this object subscriber, Action<T> handler)
        {
            IDisposable subscription = null;
            subscription = EventBus.Subscribe<T>(evt =>
            {
                try
                {
                    handler(evt);
                }
                finally
                {
                    subscription?.Dispose();
                }
            });
            return subscription;
        }

        public static IDisposable SubscribeUntil<T>(this object subscriber, Func<T, bool> condition, Action<T> handler)
        {
            IDisposable subscription = null;
            subscription = EventBus.Subscribe<T>(evt =>
            {
                if (condition(evt))
                {
                    handler(evt);
                    subscription?.Dispose();
                }
            });
            return subscription;
        }
    }
}