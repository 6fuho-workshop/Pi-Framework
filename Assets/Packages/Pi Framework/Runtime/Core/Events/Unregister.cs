using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public interface IUnRegister
    {
        void UnRegister();
        bool isEmpty { get; set; }
    }

    public interface IUnRegisterList
    {
        List<IUnRegister> unregisterList { get; }
    }

    public class UnRegisterList : IUnRegisterList
    {
        public List<IUnRegister> unregisterList { get; } = new();
    }

    public static class IUnRegisterListExtension
    {
        public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unregisterList)
        {
            if(!self.isEmpty)
                unregisterList.unregisterList.Add(self);
        }

        public static void UnregisterAll(this IUnRegisterList self)
        {
            self.unregisterList.ForEach(x => x.UnRegister());
            self.unregisterList.Clear();
        }
    }

    public struct Unregister : IUnRegister
    {
        private Action onUnRegister { get; set; }
        public bool isEmpty { get; set; }

        public Unregister(Action onUnRegister) {
            this.onUnRegister = onUnRegister; 
            isEmpty = false;
        }

        public void UnRegister()
        {
            onUnRegister.Invoke();
            onUnRegister = null;
        }
    }

    public class UnregisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> unbinders = new HashSet<IUnRegister>();

        public void AddUnregister(IUnRegister unregister) => unbinders.Add(unregister);

        public void RemoveUnregister(IUnRegister unregister) => unbinders.Remove(unregister);

        private void OnDestroy()
        {
            foreach (var ur in unbinders)
            {
                ur.UnRegister();
            }

            unbinders.Clear();
        }
    }

    public static class IUnRegisterExtension
    {
        public static IUnRegister UnregisterWhenGameObjectDestroyed(this IUnRegister self, UnityEngine.GameObject gameObject)
        {
            if (!self.isEmpty)
            {
                var trigger = gameObject.GetComponent<UnregisterOnDestroyTrigger>();

                if (!trigger)
                {
                    trigger = gameObject.AddComponent<UnregisterOnDestroyTrigger>();
                }

                trigger.AddUnregister(self);
            }

            return self;
        }

        public static IUnRegister UnregisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component =>
            self.UnregisterWhenGameObjectDestroyed(component.gameObject);
    }
}