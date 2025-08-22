using PF.Contracts;
using System;

namespace PF.DI
{
    /// <summary>
    /// Specifies the lifetime of a registered service within the service registry.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// A single instance is created and shared for the lifetime of the registry.
        /// </summary>
        Singleton,

        /// <summary>
        /// A single instance is created and shared within a defined scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// A new instance is created each time the service is requested.
        /// </summary>
        Transient
    }

    #region Event Args

    /// <summary>
    /// Event arguments for when a service is registered.
    /// </summary>
    public sealed class ServiceRegisteredEventArgs : EventArgs
    {
        public ServiceRegisteredEventArgs(Type serviceType, object instance)
        { ServiceType = serviceType; Instance = instance; }
        public Type ServiceType { get; }
        public object Instance { get; }
    }

    /// <summary>
    /// Event arguments for when a service is unregistered.
    /// </summary>
    public sealed class ServiceUnregisteredEventArgs : EventArgs
    {
        public ServiceUnregisteredEventArgs(Type serviceType, object instance)
        { ServiceType = serviceType; Instance = instance; }
        public Type ServiceType { get; }
        public object Instance { get; }
    }

    #endregion

    /// <summary>
    /// Provides a registry interface for managing the lifecycle of services.
    /// Supports registration, replacement, removal, factory registration, and clearing of services, as well as lifecycle events.
    /// </summary>
    public interface IServiceRegistry : IReadOnlyServiceLocator
    {
        /// <summary>
        /// Occurs when a new service is registered.
        /// </summary>
        event EventHandler<ServiceRegisteredEventArgs> ServiceRegistered;

        /// <summary>
        /// Occurs when a service is unregistered (removed).
        /// </summary>
        event EventHandler<ServiceUnregisteredEventArgs> ServiceUnregistered;

        /// <summary>
        /// Registers a new service instance of the specified type.
        /// Throws if a service of the same type is already registered.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="instance">The service instance to register.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the service.</returns>
        /// <exception cref="ArgumentException">Thrown if a service of the same type is already registered.</exception>
        IUnregister Register<T>(T instance) where T : class;

        /// <summary>
        /// Registers a new service instance for the specified type.
        /// Throws if a service of the same type is already registered.
        /// </summary>
        /// <param name="serviceType">The type to associate with the service.</param>
        /// <param name="instance">The service instance to register.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the service.</returns>
        /// <exception cref="ArgumentException">Thrown if a service of the same type is already registered.</exception>
        IUnregister Register(Type serviceType, object instance);

        /// <summary>
        /// Attempts to register a new service instance of the specified type.
        /// Does not throw if already registered; returns false instead.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="instance">The service instance to register.</param>
        /// <returns>True if registration succeeded; false if already registered.</returns>
        bool TryRegister<T>(T instance) where T : class;

        /// <summary>
        /// Attempts to register a new service instance for the specified type.
        /// Does not throw if already registered; returns false instead.
        /// </summary>
        /// <param name="serviceType">The type to associate with the service.</param>
        /// <param name="instance">The service instance to register.</param>
        /// <returns>True if registration succeeded; false if already registered.</returns>
        bool TryRegister(Type serviceType, object instance);

        /// <summary>
        /// Replaces the existing service of the specified type with a new instance,
        /// or registers it if not present. If a previous instance exists and <paramref name="disposeInstance"/> is true,
        /// it will be disposed if it implements <see cref="IDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to replace or register.</typeparam>
        /// <param name="instance">The new service instance.</param>
        /// <param name="disposeInstance">
        /// If true, disposes the previous instance (if any) if it implements <see cref="IDisposable"/>.
        /// </param>
        void Replace<T>(T instance, bool disposeInstance = true) where T : class;

        /// <summary>
        /// Replaces the existing service for the specified type with a new instance,
        /// or registers it if not present. If a previous instance exists and <paramref name="disposeOld"/> is true,
        /// it will be disposed if it implements <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="serviceType">The type to associate with the service.</param>
        /// <param name="instance">The new service instance.</param>
        /// <param name="disposeOld">
        /// If true, disposes the previous instance (if any) if it implements <see cref="IDisposable"/>.
        /// </param>
        void Replace(Type serviceType, object instance, bool disposeOld = true);

        /// <summary>
        /// Removes the service of the specified type if it exists.
        /// If the service instance implements <see cref="IDisposable"/> and <paramref name="disposeInstance"/> is true,
        /// it will be disposed before removal.
        /// </summary>
        /// <typeparam name="T">The type of the service to remove.</typeparam>
        /// <param name="disposeInstance">
        /// If true, disposes the instance (if any) if it implements <see cref="IDisposable"/>.
        /// </param>
        /// <returns>True if the service was removed; otherwise, false.</returns>
        bool Remove<T>(bool disposeInstance = true) where T : class;

        /// <summary>
        /// Removes the service for the specified type if it exists.
        /// If the service instance implements <see cref="IDisposable"/> and <paramref name="disposeInstance"/> is true,
        /// it will be disposed before removal.
        /// </summary>
        /// <param name="serviceType">The type of the service to remove.</param>
        /// <param name="disposeInstance">
        /// If true, disposes the instance (if any) if it implements <see cref="IDisposable"/>.
        /// </param>
        /// <returns>True if the service was removed; otherwise, false.</returns>
        bool Remove(Type serviceType, bool disposeInstance = true);

        /// <summary>
        /// Registers a factory method to create an instance of the specified type when resolved.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="factory">The factory method to create the service instance.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the factory.</returns>
        IUnregister RegisterFactory<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Registers a factory method to create an instance for the specified type when resolved.
        /// </summary>
        /// <param name="serviceType">The type to associate with the factory.</param>
        /// <param name="factory">The factory method to create the service instance.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the factory.</returns>
        IUnregister RegisterFactory(Type serviceType, Func<object> factory);

        /// <summary>
        /// Registers a factory method with context to create an instance of the specified type when resolved.
        /// The factory receives the current registry as a parameter.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="factory">The factory method with registry context.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the factory.</returns>
        IUnregister RegisterFactory<T>(Func<IServiceRegistry, T> factory) where T : class;

        /// <summary>
        /// Registers a factory method with context to create an instance for the specified type when resolved.
        /// The factory receives the current registry as a parameter.
        /// </summary>
        /// <param name="serviceType">The type to associate with the factory.</param>
        /// <param name="factory">The factory method with registry context.</param>
        /// <returns>An <see cref="IUnregister"/> token for unregistering the factory.</returns>
        IUnregister RegisterFactory(Type serviceType, Func<IServiceRegistry, object> factory);

        /// <summary>
        /// Attempts to register a factory method for the specified type.
        /// Does not throw if already registered; returns false instead.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="factory">The factory method to create the service instance.</param>
        /// <returns>True if registration succeeded; false if already registered.</returns>
        bool TryRegisterFactory<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Attempts to register a factory method for the specified type.
        /// Does not throw if already registered; returns false instead.
        /// </summary>
        /// <param name="serviceType">The type to associate with the factory.</param>
        /// <param name="factory">The factory method to create the service instance.</param>
        /// <returns>True if registration succeeded; false if already registered.</returns>
        bool TryRegisterFactory(Type serviceType, Func<object> factory);

        /// <summary>
        /// Removes the factory for the specified type if it exists.
        /// </summary>
        /// <typeparam name="T">The type of the service factory to remove.</typeparam>
        /// <returns>True if the factory was removed; otherwise, false.</returns>
        bool RemoveFactory<T>() where T : class;

        /// <summary>
        /// Removes the factory for the specified type if it exists.
        /// </summary>
        /// <param name="serviceType">The type of the service factory to remove.</param>
        /// <returns>True if the factory was removed; otherwise, false.</returns>
        bool RemoveFactory(Type serviceType);

        /// <summary>
        /// Ensures that an instance of the specified type is created and registered.
        /// If not already present, the factory is invoked immediately.
        /// </summary>
        /// <typeparam name="T">The type of the service to ensure.</typeparam>
        /// <returns>The ensured service instance.</returns>
        T Ensure<T>() where T : class;

        /// <summary>
        /// Ensures that an instance for the specified type is created and registered.
        /// If not already present, the factory is invoked immediately.
        /// </summary>
        /// <param name="serviceType">The type of the service to ensure.</param>
        /// <returns>The ensured service instance.</returns>
        object Ensure(Type serviceType);

        /// <summary>
        /// Removes all registered services and factories.
        /// Optionally disposes instances that implement <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="disposeInstances">If true, disposes all <see cref="IDisposable"/> service instances.</param>
        /// <param name="removeFactories">If true, removes all registered factories.</param>
        void Clear(bool disposeInstances = true, bool removeFactories = true);
    }
}