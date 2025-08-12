using System;
using System.Collections.Generic;

namespace PF.Core.Services
{
    /// <summary>
    /// Exposes a read-only interface for resolving and querying registered services.
    /// Does not allow registration or removal of services.
    /// </summary>
    public interface IReadOnlyServiceLocator
    {
        /// <summary>
        /// Resolves and returns an instance of the specified service type.
        /// Throws if the service is not registered.
        /// </summary>
        /// <typeparam name="T">Type of the service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        /// <exception cref="InvalidOperationException">If the service is not registered.</exception>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolves and returns an instance of the specified service type.
        /// Throws if the service is not registered.
        /// </summary>
        /// <param name="serviceType">Type of the service to resolve.</param>
        /// <returns>The resolved service instance.</returns>
        /// <exception cref="InvalidOperationException">If the service is not registered.</exception>
        object Resolve(Type serviceType);

        /// <summary>
        /// Attempts to resolve the specified service type.
        /// Does not throw if the service is not found.
        /// </summary>
        /// <typeparam name="T">Type of the service to resolve.</typeparam>
        /// <param name="instance">The resolved service instance if found; otherwise, null.</param>
        /// <returns>True if the service was found; otherwise, false.</returns>
        bool TryResolve<T>(out T instance) where T : class;

        /// <summary>
        /// Attempts to resolve the specified service type.
        /// Does not throw if the service is not found.
        /// </summary>
        /// <param name="serviceType">Type of the service to resolve.</param>
        /// <param name="instance">The resolved service instance if found; otherwise, null.</param>
        /// <returns>True if the service was found; otherwise, false.</returns>
        bool TryResolve(Type serviceType, out object instance);

        /// <summary>
        /// Checks if a service of the specified type is registered (instance or factory).
        /// </summary>
        /// <typeparam name="T">Type of the service to check.</typeparam>
        /// <returns>True if the service is registered; otherwise, false.</returns>
        bool Contains<T>() where T : class;

        /// <summary>
        /// Checks if a service of the specified type is registered (instance or factory).
        /// </summary>
        /// <param name="serviceType">Type of the service to check.</param>
        /// <returns>True if the service is registered; otherwise, false.</returns>
        bool Contains(Type serviceType);

        /// <summary>
        /// Checks if an instance of the specified type is registered (not factory).
        /// </summary>
        /// <typeparam name="T">Type of the service to check.</typeparam>
        /// <returns>True if an instance is registered; otherwise, false.</returns>
        bool HasInstance<T>() where T : class;

        /// <summary>
        /// Checks if an instance of the specified type is registered (not factory).
        /// </summary>
        /// <param name="serviceType">Type of the service to check.</param>
        /// <returns>True if an instance is registered; otherwise, false.</returns>
        bool HasInstance(Type serviceType);

        /// <summary>
        /// Checks if a factory for the specified type is registered (not instance).
        /// </summary>
        /// <typeparam name="T">Type of the service to check.</typeparam>
        /// <returns>True if a factory is registered; otherwise, false.</returns>
        bool HasFactory<T>() where T : class;

        /// <summary>
        /// Checks if a factory for the specified type is registered (not instance).
        /// </summary>
        /// <param name="serviceType">Type of the service to check.</param>
        /// <returns>True if a factory is registered; otherwise, false.</returns>
        bool HasFactory(Type serviceType);

        /// <summary>
        /// Gets all registered service types (instances and/or factories).
        /// </summary>
        /// <returns>An enumerable of registered service types.</returns>
        IEnumerable<Type> GetRegisteredServiceTypes();
    }
}
