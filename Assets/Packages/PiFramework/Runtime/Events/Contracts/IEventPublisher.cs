using System;

namespace PF.Contracts
{
    /// <summary>
    /// Non-generic event publisher interface.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        void Publish();
    }

    /// <summary>
    /// Generic event publisher interface for one argument.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    public interface IEventPublisher<T>
    {
        /// <summary>
        /// Publishes an event with one argument.
        /// </summary>
        /// <param name="t">Event argument.</param>
        void Publish(T t);
    }

    /// <summary>
    /// Generic event publisher interface for two arguments.
    /// </summary>
    /// <typeparam name="T">Type of the first event argument.</typeparam>
    /// <typeparam name="K">Type of the second event argument.</typeparam>
    public interface IEventPublisher<T, K>
    {
        /// <summary>
        /// Publishes an event with two arguments.
        /// </summary>
        /// <param name="t">First event argument.</param>
        /// <param name="k">Second event argument.</param>
        void Publish(T t, K k);
    }

    /// <summary>
    /// Generic event publisher interface for three arguments.
    /// </summary>
    /// <typeparam name="T">Type of the first event argument.</typeparam>
    /// <typeparam name="K">Type of the second event argument.</typeparam>
    /// <typeparam name="S">Type of the third event argument.</typeparam>
    public interface IEventPublisher<T, K, S>
    {
        /// <summary>
        /// Publishes an event with three arguments.
        /// </summary>
        /// <param name="t">First event argument.</param>
        /// <param name="k">Second event argument.</param>
        /// <param name="s">Third event argument.</param>
        void Publish(T t, K k, S s);
    }
}