using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{ 
    /// <summary>
    /// Defines an interface for objects that can be unregistered from event systems.
    /// Provides a mechanism for cleaning up event subscriptions or other resources.
    /// Instead of directly calling an unregister method, this approach encapsulates
    /// the unregistration logic into a separate object that can be invoked at an 
    /// appropriate time, enabling better control over resource cleanup.
    /// </summary>
    public interface IUnregister
    {
        /// <summary>
        /// Performs the unregistration action, removing event listeners or cleaning up resources.
        /// </summary>
        void Unregister();

        /// <summary>
        /// Indicates whether this unregister instance is empty (has no actual unregistration work).
        /// </summary>
        bool IsEmpty { get; set; }
    }

    /// <summary>
    /// Defines a container that holds multiple IUnregister instances for batch management.
    /// See the UnregisterOnDestroyTrigger class for more information about usage examples.
    /// </summary>
    public interface IUnregisterList
    {
        /// <summary>
        /// Collection of unregisterable objects that can be processed together.
        /// </summary>
        List<IUnregister> Items { get; }
    }

    /// <summary>
    /// Standard implementation of IUnregisterList interface.
    /// Provides a container for managing multiple unregisterable objects.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example of managing multiple subscriptions together
    /// var unregisterList = new UnregisterList();
    /// 
    /// // Later when cleaning up
    /// IUnregisterListExtension.UnregisterAll(unregisterList);
    /// </code>
    /// </example>
    public class UnregisterList : IUnregisterList
    {
        /// <summary>
        /// Collection of unregisterable objects.
        /// </summary>
        public List<IUnregister> Items { get; } = new();
        
    }

    /// <summary>
    /// Extension methods for the IUnregisterList interface.
    /// Provides utilities for managing collections of unregistrable objects.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create a list to manage event subscriptions
    /// var unregisterList = new UnregisterList();
    /// 
    /// // Add event subscriptions to the list
    /// EventSystem.OnLevelStart.Subscribe(OnLevelStart).AddToPendingList(unregisterList);
    /// EventSystem.OnLevelEnd.Subscribe(OnLevelEnd).AddToPendingList(unregisterList);
    /// EventSystem.OnPlayerDeath.Subscribe(OnPlayerDeath).AddToPendingList(unregisterList);
    /// 
    /// // When done with all subscriptions (e.g. when changing scenes)
    /// unregisterList.UnregisterAll();
    /// </code>
    /// </example>
    public static class IUnregisterListExtension
    {
        /// <summary>
        /// Add to pending unregisterList for later unregister
        /// </summary>
        /// <param name="self">The unregistrable object to add to the list</param>
        /// <param name="unRegisterList">The collection to add the unregistrable to</param>
        public static void AddToPendingList(this IUnregister self, IUnregisterList unRegisterList)
        {
            if(!self.IsEmpty)
                unRegisterList.Items.Add(self);
        }

        /// <summary>
        /// Unregisters all items in the list and clears the collection.
        /// This is useful for batch cleanup operations, such as when transitioning scenes
        /// or shutting down a subsystem.
        /// </summary>
        /// <param name="self">The collection of unregistrables to process</param>
        public static void UnregisterAll(this IUnregisterList self)
        {
            self.Items.ForEach(x => x.Unregister());
            self.Items.Clear();
        }
    }

    /// <summary>
    /// A simple implementation of IUnregister that executes a custom action when unregistered.
    /// Useful for creating one-off unregistration logic without creating a dedicated class.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create a custom unregistration action
    /// var subscription = SomeEventSystem.Subscribe(OnEvent);
    /// var customUnregister = new CustomUnregister(() => {
    ///     Debug.Log("Unregistering event subscription");
    ///     SomeEventSystem.Unsubscribe(subscription);
    /// });
    /// 
    /// // Later, when you need to unregister
    /// customUnregister.Unregister();
    /// </code>
    /// </example>
    public struct CustomUnregister : IUnregister
    {
        /// <summary>
        /// The action to be executed during unregistration.
        /// </summary>
        private Action onUnregister { get; set; }
        
        /// <summary>
        /// Indicates whether this unregister instance has any work to perform.
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Creates a new CustomUnregister with the specified unregistration handler.
        /// </summary>
        /// <param name="onUnregister">Action to execute when Unregister is called</param>
        public CustomUnregister(Action onUnregister) {
            this.onUnregister = onUnregister; 
            IsEmpty = false;
        }

        /// <summary>
        /// Executes the stored unregistration action and cleans up the reference.
        /// </summary>
        public void Unregister()
        {
            onUnregister.Invoke();
            onUnregister = null;
        }
    }

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
        public static IUnregister UnregisterWhenDestroyed(this IUnregister self, UnityEngine.GameObject gameObject)
        {
            if (!self.IsEmpty)
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
        public static IUnregister UnregisterWhenGameObjectDestroyed<T>(this IUnregister self, T component)
            where T : UnityEngine.Component =>
            self.UnregisterWhenDestroyed(component.gameObject);
    }
}