using PF.Contracts;
using System;

namespace PF.Contracts
{
    /// <summary>
    /// Defines the contract for a type-based event bus, enabling systems to communicate via events
    /// without direct dependencies. Provides APIs for publishing, subscribing, and managing event handlers.
    /// </summary>
    public interface IEventBus : IDisposable
    {
        /// <summary>
        /// Publishes an event of type T by creating a new instance and dispatching it.
        /// Does nothing if no subscriber exists for this event type.
        /// </summary>
        void SendEvent<T>() where T : new();

        /// <summary>
        /// Publishes a pre-constructed event of type T.
        /// Does nothing if no subscriber exists for this event type.
        /// </summary>
        void SendEvent<T>(T e);

        /// <summary>
        /// Registers a callback to be invoked when an event of type T is published.
        /// Automatically creates the event type if it does not exist.
        /// </summary>
        IUnregister Subscribe<T>(Action<T> callback);

        /// <summary>
        /// Unregisters a callback from being invoked when an event of type T is published.
        /// Does nothing if the event type does not exist.
        /// </summary>
        void Unsubscribe<T>(Action<T> callback);

        /// <summary>
        /// Checks if an event type has been created (has at least one subscriber).
        /// </summary>
        bool HasEvent<T>();
    }
}