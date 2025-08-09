using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF
{
    /// <summary>
    /// A type-based event dispatcher that allows systems to communicate without direct dependencies.
    /// The EventBus maintains a registry of event types and their subscribers, facilitating 
    /// loosely coupled communication between components.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create an event bus
    /// var eventBus = new EventBus();
    /// 
    /// // Subscribe to an event
    /// IUnregister subscription = eventBus.Subscribe&lt;PlayerDeathEvent&gt;(OnPlayerDeath);
    /// 
    /// // Publish an event
    /// eventBus.Send(new PlayerDeathEvent { Position = player.transform.position });
    /// 
    /// // Later, unsubscribe
    /// subscription.Unregister();
    /// // Or alternatively:
    /// eventBus.Unsubscribe&lt;PlayerDeathEvent&gt;(OnPlayerDeath);
    /// </code>
    /// </example>
    public class EventBus
    {
        /// <summary>
        /// Registry of event types to their event handlers.
        /// </summary>
        private readonly Dictionary<Type, PiEventBase> typeEvents = new();

        /// <summary>
        /// Publishes an event of type T by creating a new instance and dispatching it.
        /// </summary>
        /// <typeparam name="T">The event type to send (must have a parameterless constructor)</typeparam>
        public void SendEvent<T>() where T : new() => GetEvent<PiEvent<T>>()?.Invoke(new T());

        /// <summary>
        /// Publishes a pre-constructed event of type T.
        /// </summary>
        /// <typeparam name="T">The event type to send</typeparam>
        /// <param name="e">The event instance to publish</param>
        public void SendEvent<T>(T e) => GetEvent<PiEvent<T>>()?.Invoke(e);

        /// <summary>
        /// Registers a callback to be invoked when an event of type T is published.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to</typeparam>
        /// <param name="callback">The action to invoke when the event is published</param>
        /// <returns>An IUnregister that can be used to unsubscribe</returns>
        public IUnregister Subscribe<T>(Action<T> callback) => GetOrAddEvent<PiEvent<T>>().Register(callback);

        /// <summary>
        /// Unregisters a callback from being invoked when an event of type T is published.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from</typeparam>
        /// <param name="callback">The action to unregister</param>
        public void Unsubscribe<T>(Action<T> callback)
        {
            var e = GetEvent<PiEvent<T>>();
            e?.Unregister(callback);
        }

        /// <summary>
        /// Removes all event subscriptions from this event bus.
        /// </summary>
        internal void Clear()
        {
            foreach (var piEvent in typeEvents.Values)
            {
                piEvent.UnregisterAll();
            }
            typeEvents.Clear();
        }

        /// <summary>
        /// Retrieves an existing event handler for the specified type if one exists.
        /// </summary>
        /// <typeparam name="T">The event handler type</typeparam>
        /// <returns>The event handler instance or null if not found</returns>
        T GetEvent<T>() where T : PiEventBase
        {
            return typeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        /// <summary>
        /// Retrieves an existing event handler for the specified type or creates a new one if none exists.
        /// </summary>
        /// <typeparam name="T">The event handler type</typeparam>
        /// <returns>The existing or newly created event handler instance</returns>
        T GetOrAddEvent<T>() where T : PiEventBase, new()
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

    /// <summary>
    /// Interface for classes that want to handle specific event types.
    /// Implementing this interface provides a standardized way to receive events
    /// and enables the use of extension methods for easier event subscription management.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to handle</typeparam>
    /// <example>
    /// <code>
    /// public class MyEventHandler : MonoBehaviour, IEventSubscriber&lt;PlayerDiedEvent&gt;
    /// {
    ///     private IUnregister subscription;
    ///     
    ///     private void OnEnable()
    ///     {
    ///         subscription = this.SubscribeEvent();
    ///     }
    ///     
    ///     private void OnDisable()
    ///     {
    ///         subscription.Unregister();
    ///     }
    ///     
    ///     public void EventHandler(PlayerDiedEvent e)
    ///     {
    ///         Debug.Log($"Player died at position: {e.Position}");
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IEventSubscriber<TEvent>
    {
        /// <summary>
        /// Method that will be called when an event of type TEvent is published.
        /// </summary>
        /// <param name="e">The event data</param>
        void HandleEvent(TEvent e);
    }

    /// <summary>
    /// Extension methods for the IEventSubscriber interface.
    /// These methods simplify the process of subscribing to global events through PiBase.
    /// </summary>
    public static class ISubscriberExtension
    {
        /// <summary>
        /// Subscribes the implementing class to receive events of type T through PiBase.typeEvents.
        /// The EventHandler method will be called when events of that type are published.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to</typeparam>
        /// <param name="self">The implementing class instance</param>
        /// <returns>An IUnregister that can be used to unsubscribe</returns>
        /// <example>
        /// <code>
        /// // In a MonoBehaviour that implements IEventSubscriber&lt;GameStartEvent&gt;
        /// private IUnregister subscription;
        /// 
        /// void OnEnable()
        /// {
        ///     subscription = this.SubscribeEvent();
        /// }
        /// 
        /// void OnDisable()
        /// {
        ///     subscription.Unregister();
        /// }
        /// </code>
        /// </example>
        public static IUnregister SubscribeEvent<T>(this IEventSubscriber<T> self)
        {
            return PiBase.TypeEvents.Subscribe<T>(self.HandleEvent);
        }

        /// <summary>
        /// Unsubscribes the implementing class from receiving events of type T through PiBase.typeEvents.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from</typeparam>
        /// <param name="self">The implementing class instance</param>
        public static void Unsubscribe<T>(this IEventSubscriber<T> self)
        {
            PiBase.TypeEvents.Unsubscribe<T>(self.HandleEvent);
        }
    }
}