using PF.Primitives;
using PF;
using PF.Contracts;

namespace PF.Events
{
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
            return Pi.EventBus.Subscribe<T>(self.HandleEvent);
        }

        /// <summary>
        /// Unsubscribes the implementing class from receiving events of type T through PiBase.typeEvents.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from</typeparam>
        /// <param name="self">The implementing class instance</param>
        public static void Unsubscribe<T>(this IEventSubscriber<T> self)
        {
            Pi.EventBus.Unsubscribe<T>(self.HandleEvent);
        }
    }
}