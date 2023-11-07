using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public interface IUnRegister
    {
        void ExecUnRegister();
    }

    public interface IUnRegisterList
    {
        List<IUnRegister> unRegisterList { get; }
    }

    public static class IUnRegisterListExtension
    {
        public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList) =>
            unRegisterList.unRegisterList.Add(self);

        public static void UnRegisterAll(this IUnRegisterList self)
        {
            self.unRegisterList.ForEach(x => x.ExecUnRegister());
            self.unRegisterList.Clear();
        }
    }

    public struct UnRegister : IUnRegister
    {
        private Action unRegister { get; set; }
        public UnRegister(Action unRegisterAction) => unRegister = unRegisterAction;

        void IUnRegister.ExecUnRegister()
        {
            unRegister.Invoke();
            unRegister = null;
        }
    }

    public class UnRegisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        private readonly HashSet<IUnRegister> unRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister) => unRegisters.Add(unRegister);

        public void RemoveUnRegister(IUnRegister unRegister) => unRegisters.Remove(unRegister);

        private void OnDestroy()
        {
            foreach (var unRegister in unRegisters)
            {
                unRegister.ExecUnRegister();
            }

            unRegisters.Clear();
        }
    }

    public static class UnRegisterExtension
    {
        public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, UnityEngine.GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<UnRegisterOnDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterOnDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);

            return unRegister;
        }

        public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : UnityEngine.Component =>
            self.UnRegisterWhenGameObjectDestroyed(component.gameObject);
    }
}