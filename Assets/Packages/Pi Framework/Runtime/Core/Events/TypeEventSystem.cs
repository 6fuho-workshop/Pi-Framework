using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class TypeEventSystem
    {
        private readonly Dictionary<Type, IPiEvent> typeEvents = new();
        public void Send<T>() where T : new() => GetEvent<PiEvent<T>>()?.Invoke(new T());

        public void Send<T>(T e) => GetEvent<PiEvent<T>>()?.Invoke(e);

        public IUnRegister AddListener<T>(Action<T> onEvent) => GetOrAddEvent<PiEvent<T>>().AddListener(onEvent);

        public void RemoveListener<T>(Action<T> onEvent)
        {
            var e = GetEvent<PiEvent<T>>();
            e?.RemoveListener(onEvent);
        }

        public void RemoveAllListeners<T>() where T : IPiEvent
        {
            GetEvent<T>()?.RemoveAllListeners();
        }

        internal void Clear()
        {
            foreach (var piEvent in typeEvents.Values)
            {
                piEvent.RemoveAllListeners();
            }
            typeEvents.Clear();
        }

        T GetEvent<T>() where T : IPiEvent
        {
            return typeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        T GetOrAddEvent<T>() where T : IPiEvent, new()
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