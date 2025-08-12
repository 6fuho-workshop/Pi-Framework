using PF;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Core.Common {
    /// <summary>
    /// Interface for unregistering callbacks.
    /// </summary>
    public interface IUnregister
    {
        /// <summary>
        /// Unregister the callback.
        /// </summary>
        void Unregister();

        /// <summary>
        /// Indicates whether the registration is active (the callback is still registered).
        /// </summary>
        bool IsRegistered { get; }
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

        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Creates a new CustomUnregister with the specified unregistration handler.
        /// </summary>
        /// <param name="unregister">Action to execute when Unregister is called</param>
        public CustomUnregister(Action unregister, bool isRegistered = true)
        {
            IsRegistered = isRegistered;
            onUnregister = isRegistered ? unregister : null;
        }

        /// <summary>
        /// Executes the stored unregistration action and cleans up the reference.
        /// </summary>
        public void Unregister()
        {
            if (IsRegistered)
            {
                onUnregister.Invoke();
                onUnregister = null;
                IsRegistered = false;
            }
        }
    }
}