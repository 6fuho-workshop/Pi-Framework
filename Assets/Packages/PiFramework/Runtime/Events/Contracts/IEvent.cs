using System;

namespace PF.Contracts
{
    public interface IEvent
    {
        /// <summary>
        /// Register callback
        /// </summary>
        IUnregister Register(Action callback);

        /// <summary>
        /// Remove callback
        /// </summary>
        /// <param name="callback"></param>
        void Unregister(Action callback);
    }

    /// <summary>
    /// Generic event interface with one argument.
    /// </summary>
    /// <typeparam name="T">Type of the event argument.</typeparam>
    public interface IEvent<T>
    {
        /// <summary>
        /// Register callback with one argument.
        /// </summary>
        IUnregister Register(Action<T> callback);

        /// <summary>
        /// Remove callback.
        /// </summary>
        /// <param name="callback"></param>
        void Unregister(Action<T> callback);
    }

    /// <summary>
    /// Generic event interface with two arguments.
    /// </summary>
    /// <typeparam name="T">Type of the first event argument.</typeparam>
    /// <typeparam name="K">Type of the second event argument.</typeparam>
    public interface IEvent<T, K>
    {
        /// <summary>
        /// Register callback with two arguments.
        /// </summary>
        IUnregister Register(Action<T, K> callback);

        /// <summary>
        /// Remove callback.
        /// </summary>
        /// <param name="callback"></param>
        void Unregister(Action<T, K> callback);
    }

    /// <summary>
    /// Generic event interface with three arguments.
    /// </summary>
    /// <typeparam name="T">Type of the first event argument.</typeparam>
    /// <typeparam name="K">Type of the second event argument.</typeparam>
    /// <typeparam name="S">Type of the third event argument.</typeparam>
    public interface IEvent<T, K, S>
    {
        /// <summary>
        /// Register callback with three arguments.
        /// </summary>
        IUnregister Register(Action<T, K, S> callback);

        /// <summary>
        /// Remove callback.
        /// </summary>
        /// <param name="callback"></param>
        void Unregister(Action<T, K, S> callback);
    }
}