using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class EventBus
    {
        private readonly Dictionary<Type, PiEventBase> typeEvents = new();
        public void SendEvent<T>() where T : new() => GetEvent<PiEvent<T>>()?.Invoke(new T());

        public void SendEvent<T>(T e) => GetEvent<PiEvent<T>>()?.Invoke(e);

        /// <summary>
        /// Add event callback
        /// </summary>
        public IUnRegister SubscribeEvent<T>(Action<T> callback) => GetOrAddEvent<PiEvent<T>>().Register(callback);

        public void Unsubscribe<T>(Action<T> callback)
        {
            var e = GetEvent<PiEvent<T>>();
            e?.UnRegister(callback);
        }

        internal void Clear()
        {
            foreach (var piEvent in typeEvents.Values)
            {
                piEvent.UnRegisterAll();
            }
            typeEvents.Clear();
        }

        T GetEvent<T>() where T : PiEventBase
        {
            return typeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        T GetOrAddEvent<T>() where T : PiEventBase, new()
        {
            var eType = typeof(T);
            if (typeEvents.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            typeEvents.Add(eType, t);
            return t;
        }
    }

    /// <summary>Global events</summary>
    public interface IEventSubscriber<TEvent>
    {
        void EventHandler(TEvent e);
    }

    public static class ISubscriberExtension
    {
        public static IUnRegister SubscribeEvent<T>(this IEventSubscriber<T> self)
        {
            return PiBase.typeEvents.SubscribeEvent<T>(self.EventHandler);
        }

        public static void Unsubscribe<T>(this IEventSubscriber<T> self)
        {
            PiBase.typeEvents.Unsubscribe<T>(self.EventHandler);
        }
    }
}