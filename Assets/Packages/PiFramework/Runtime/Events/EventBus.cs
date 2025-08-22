using PF.Contracts;
using PF.Primitives;
using PF.Utils;
using System;
using System.Collections.Concurrent;

namespace PF.Events
{
    /// <summary>
    /// Implements a thread-safe type-based event bus for decoupled event communication.
    /// </summary>
    public class EventBus : IEventBus, IDisposable
    {
        /// <summary>
        /// Stores event instances by their type for dispatching and subscription.
        /// Value type is <see cref="IDisposable"/> to allow unified cleanup (Dispose) of all events in <see cref="Dispose"/> and <see cref="Clear"/>.
        /// Only <c>PiEvent&lt;T&gt;</c> (which implements <see cref="IDisposable"/>) is ever added, so type safety is guaranteed.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IDisposable> typeEvents = new();

        private bool _disposed;

        /// <inheritdoc/>
        public void SendEvent<T>() where T : new()
        {
            this.ThrowIfDisposed(_disposed);
            GetEvent<PiEvent<T>>()?.Publish(new T());
        }

        /// <inheritdoc/>
        public void SendEvent<T>(T e)
        {
            this.ThrowIfDisposed(_disposed);
            GetEvent<PiEvent<T>>()?.Publish(e);
        }

        /// <inheritdoc/>
        public IUnregister Subscribe<T>(Action<T> callback)
        {
            this.ThrowIfDisposed(_disposed);
            return GetOrAddEvent<PiEvent<T>>().Register(callback);
        }

        /// <inheritdoc/>
        public void Unsubscribe<T>(Action<T> callback)
        {
            if (_disposed) return;
            var e = GetEvent<PiEvent<T>>();
            e?.Unregister(callback);
        }

        /// <summary>
        /// Determines if an event of type <typeparamref name="T"/> is present in the bus.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns><c>true</c> if the event is present; otherwise, <c>false</c>.</returns>
        public bool HasEvent<T>()
        {
            return typeEvents.ContainsKey(typeof(PiEvent<T>));
        }

        /// <summary>
        /// Gets the event instance of the specified type, or default if not present.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>The event instance or default.</returns>
        private TEvent GetEvent<TEvent>()
        {
            if (_disposed) return default;
            return typeEvents.TryGetValue(typeof(TEvent), out var e) ? (TEvent)e : default;
        }

        /// <summary>
        /// Gets the event instance of the specified type, or creates and adds it if not present.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>The event instance.</returns>
        private TEvent GetOrAddEvent<TEvent>() where TEvent : IDisposable, new()
        {
            // Only called from Subscribe, already guarded
            var eType = typeof(TEvent);
            return (TEvent)typeEvents.GetOrAdd(eType, _ => new TEvent());
        }

        public virtual void Dispose()
        {
            if (_disposed) return;
            foreach (var piEvent in typeEvents.Values)
            {
                piEvent.Dispose();
            }
            typeEvents.Clear();
            _disposed = true;
        }
    }
}