using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class TypeEventSystem
    {
        private readonly Dictionary<Type, PiEventBase> typeEvents = new();
        public void Dispatch<T>() where T : new() => GetEvent<PiEvent<T>>()?.Invoke(new T());

        public void Dispatch<T>(T e) => GetEvent<PiEvent<T>>()?.Invoke(e);

        public IUnRegister Subscribe<T>(Action<T> onEvent) => GetOrAddEvent<PiEvent<T>>().Register(onEvent);

        public void Unsubscribe<T>(Action<T> onEvent)
        {
            var e = GetEvent<PiEvent<T>>();
            e?.UnRegister(onEvent);
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
}