using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PF.Core.Common;
namespace PF.Unity.Common
{ 
    /// <summary>
    /// MonoBehaviour that automatically unregisters a collection of IUnregister objects when its GameObject is destroyed.
    /// Useful for tying event registrations to the lifecycle of a GameObject.
    /// </summary>
    /// <example>
    /// <code>
    /// // This is typically used through the extension method below, but can be used directly:
    /// var trigger = gameObject.AddComponent<UnregisterOnDestroyTrigger>();
    /// trigger.AddUnregister(myEventSubscription);
    /// 
    /// // Remove if needed before destruction
    /// trigger.RemoveUnregister(myEventSubscription);
    /// </code>
    /// </example>
    public class UnregisterOnDestroyTrigger : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// Collection of unregisters that will be processed on destruction.
        /// </summary>
        private readonly HashSet<IUnregister> unRegisters = new HashSet<IUnregister>();

        /// <summary>
        /// Adds an unregister to be processed when this GameObject is destroyed.
        /// </summary>
        /// <param name="unregister">The unregister to add</param>
        public void AddUnregister(IUnregister unregister) => unRegisters.Add(unregister);

        /// <summary>
        /// Removes an unregister from the collection if it should no longer be processed on destruction.
        /// </summary>
        /// <param name="unregister">The unregister to remove</param>
        public void RemoveUnregister(IUnregister unregister) => unRegisters.Remove(unregister);

        /// <summary>
        /// Called when the GameObject is destroyed. Processes all registered unregisters.
        /// </summary>
        private void OnDestroy()
        {
            foreach (var ur in unRegisters)
            {
                ur.Unregister();
            }

            unRegisters.Clear();
        }
    }

    /// <summary>
    /// Extension methods for the IUnregister interface.
    /// Provides utilities for automatic lifecycle management of unregistrables.
    /// </summary>
    /// <example>
    /// <code>
    /// // Automatically unregister when a GameObject is destroyed
    /// public class PlayerController : MonoBehaviour
    /// {
    ///     private void Start()
    ///     {
    ///         // Subscribe to events and automatically unregister when this GameObject is destroyed
    ///         EventSystem.OnEnemySpawned.Subscribe(OnEnemySpawned)
    ///             .UnregisterWhenGameObjectDestroyed(this.gameObject);
    ///             
    ///         // Alternative using component reference
    ///         EventSystem.OnPowerupCollected.Subscribe(OnPowerupCollected)
    ///             .UnregisterWhenGameObjectDestroyed(this);
    ///     }
    ///     
    ///     private void OnEnemySpawned() { /* handle event */ }
    ///     private void OnPowerupCollected() { /* handle event */ }
    /// }
    /// </code>
    /// </example>
    public static class IUnregisterExtension
    {
        /// <summary>
        /// Sets up automatic unregistration when the specified GameObject is destroyed.
        /// </summary>
        /// <param name="self">The unregister to be processed on GameObject destruction</param>
        /// <param name="gameObject">The GameObject whose lifecycle controls the unregistration</param>
        /// <returns>The original unregister for chaining</returns>
        public static IUnregister UnregisterWhenGODestroyed(this IUnregister self, UnityEngine.GameObject gameObject)
        {
            if (self.IsRegistered)
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

        /// <summary>
        /// Sets up automatic unregistration when the GameObject containing the specified Component is destroyed.
        /// </summary>
        /// <typeparam name="T">Type of the component</typeparam>
        /// <param name="self">The unregister to be processed on GameObject destruction</param>
        /// <param name="component">The Component whose GameObject lifecycle controls the unregistration</param>
        /// <returns>The original unregister for chaining</returns>
        public static IUnregister UnregisterWhenDestroyed<T>(this IUnregister self, T component)
            where T : UnityEngine.Component =>
            self.UnregisterWhenGODestroyed(component.gameObject);
    }
}