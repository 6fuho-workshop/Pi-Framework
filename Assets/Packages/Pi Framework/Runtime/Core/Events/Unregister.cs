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
        /// <summary>
        /// Add to pending unregisterList for later unregister
        /// </summary>
        /// <param name="unRegisterList"></param>
        public static void AddToPendingList(this IUnRegister self, IUnRegisterList unRegisterList)
        {
            if(!self.isEmpty)
                unRegisterList.unregisterList.Add(self);
        }

        public static void UnRegisterAll(this IUnRegisterList self)
        {
            self.unregisterList.ForEach(x => x.UnRegister());
            self.unregisterList.Clear();
        }
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action onUnregister { get; set; }
        public bool isEmpty { get; set; }

        public CustomUnRegister(Action unregisterHandler) {
            onUnregister = unregisterHandler; 
            isEmpty = false;
        }

        public void UnRegister()
        {
            onUnregister.Invoke();
            onUnregister = null;
        }
    }

    public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> unRegisters = new HashSet<IUnRegister>();

        public void AddUnregister(IUnRegister unregister) => unRegisters.Add(unregister);

        public void RemoveUnregister(IUnRegister unregister) => unRegisters.Remove(unregister);

        private void OnDestroy()
        {
            foreach (var ur in unRegisters)
            {
                ur.UnRegister();
            }

            unRegisters.Clear();
        }
    }

    public static class IUnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister self, UnityEngine.GameObject gameObject)
        {
            if (!self.isEmpty)
            {
                var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

                if (!trigger)
                {
                    trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
                }

                trigger.AddUnregister(self);
            }

            return self;
        }

        public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component =>
            self.UnRegisterWhenGameObjectDestroyed(component.gameObject);
    }
}