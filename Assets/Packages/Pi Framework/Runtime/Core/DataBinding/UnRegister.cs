using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public interface IUnregister
    {
        void InvokeUnregister();
    }

    public interface IUnRegisterList
    {
        List<IUnregister> unregisterList { get; }
    }

    public static class IUnRegisterListExtension
    {
        public static void AddToUnregisterList(this IUnregister self, IUnRegisterList unRegisterList) =>
            unRegisterList.unregisterList.Add(self);

        public static void UnRegisterAll(this IUnRegisterList self)
        {
            self.unregisterList.ForEach(x => x.InvokeUnregister());
            self.unregisterList.Clear();
        }
    }

    public struct Unregister : IUnregister
    {
        private Action unRegister { get; set; }
        public Unregister(Action unRegisterAction) => unRegister = unRegisterAction;

        void IUnregister.InvokeUnregister()
        {
            unRegister.Invoke();
            unRegister = null;
        }
    }

    public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnregister> unRegisters = new HashSet<IUnregister>();

        public void AddUnregister(IUnregister unregister) => unRegisters.Add(unregister);

        public void RemoveUnregister(IUnregister unregister) => unRegisters.Remove(unregister);

        private void OnDestroy()
        {
            foreach (var ur in unRegisters)
            {
                ur.InvokeUnregister();
            }

            unRegisters.Clear();
        }
    }

    public static class UnRegisterExtension
    {
        public static IUnregister UnregisterWhenGameObjectDestroyed(this IUnregister unregister, UnityEngine.GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnregister(unregister);

            return unregister;
        }

        public static IUnregister UnregisterWhenGameObjectDestroyed<T>(this IUnregister self, T component)
            where T : UnityEngine.Component =>
            self.UnregisterWhenGameObjectDestroyed(component.gameObject);
    }
}