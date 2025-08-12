using PF.Core.Common;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Core.Common
{
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
            if (self.IsRegistered)
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
}