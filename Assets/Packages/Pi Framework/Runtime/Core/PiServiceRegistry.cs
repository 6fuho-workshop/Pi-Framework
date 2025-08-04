using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    /// <summary>
    /// Defines a registry system for managing and providing services. 
    /// It allows services to be registered, retrieved, and removed by type.
    /// Includes event notifications for service lifecycle changes.
    /// </summary>
    public interface IServiceRegistry : IServiceProvider
    {
        #region Events

        /// <summary>
        /// Occurs when a new service is added to the registry.
        /// </summary>
        event EventHandler<ServiceEventArgs> ServiceAdded;

        /// <summary>
        /// Occurs when a service is removed from the registry.
        /// </summary>
        event EventHandler<ServiceEventArgs> ServiceRemoved;

        #endregion

        #region Service Registration

        /// <summary>
        /// Registers a service instance of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <returns>An <see cref="IUnregister"/> token that can be used to unregister the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the service is null.</exception>
        /// <exception cref="ArgumentException">Thrown if a service of the same type is already registered.</exception>
        IUnregister AddService<T>(T service) where T : class;

        /// <summary>
        /// Registers a service instance for the specified type.
        /// </summary>
        /// <param name="type">The type to associate with the service.</param>
        /// <param name="service">The service instance to register.</param>
        /// <returns>An <see cref="IUnregister"/> token that can be used to unregister the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the service or type is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the service does not implement the type or is already registered.</exception>
        IUnregister AddService(Type type, object service);

        #endregion

        #region Service Retrieval

        /// <summary>
        /// Retrieves a registered service of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service instance if found; otherwise, null.</returns>
        T GetService<T>() where T : class;

        /// <summary>
        /// Attempts to retrieve a registered service of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <param name="service">When this method returns, contains the service instance if found; otherwise, null.</param>
        /// <returns>True if the service was found; otherwise, false.</returns>
        bool TryGetService<T>(out T service) where T : class;

        /// <summary>
        /// Attempts to retrieve a registered service by its type.
        /// </summary>
        /// <param name="type">The type of the service to retrieve.</param>
        /// <param name="service">When this method returns, contains the service instance if found; otherwise, null.</param>
        /// <returns>True if the service was found; otherwise, false.</returns>
        bool TryGetService(Type type, out object service);

        #endregion

        #region Service Removal

        /// <summary>
        /// Removes the service registered under the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type of the service to remove.</typeparam>
        void RemoveService<T>() where T : class;

        /// <summary>
        /// Removes the service registered under the specified type.
        /// </summary>
        /// <param name="type">The type of the service to remove.</param>
        void RemoveService(Type type);

        #endregion
    }


    /// <summary>
    /// Provides data for service-related events such as service addition and removal.
    /// </summary>
    public class ServiceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ServiceEventArgs class.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="serviceInstance">The service instance.</param>
        public ServiceEventArgs(Type serviceType, object serviceInstance)
        {
            ServiceType = serviceType;
            Instance = serviceInstance;
        }

        /// <summary>
        /// Gets the type of the service involved in the event.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the service instance involved in the event.
        /// </summary>
        public object Instance { get; }
    }


    /// <summary>
    /// Provides a registry for managing services by type, allowing for service registration, retrieval, and removal.
    /// </summary>
    /// <remarks>
    /// The <see cref="ServiceRegistry"/> class allows clients to register services by their type,
    /// retrieve them later, and remove them when no longer needed. It supports both generic and non-generic methods
    /// for interacting with services. Events are raised when services are added or removed, enabling subscribers to
    /// react to changes in the registry.
    /// </remarks>
    public class ServiceRegistry : IServiceRegistry
    {
        /// <summary>
        /// Dictionary of all registered services by type.
        /// </summary>
        protected readonly Dictionary<Type, object> _services = new();

        #region Events

        /// <inheritdoc />
        public event EventHandler<ServiceEventArgs> ServiceAdded;

        /// <inheritdoc />
        public event EventHandler<ServiceEventArgs> ServiceRemoved;

        /// <summary>
        /// Raises the ServiceAdded event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnServiceAdded(ServiceEventArgs e)
        {
            ServiceAdded?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the ServiceRemoved event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnServiceRemoved(ServiceEventArgs e)
        {
            ServiceRemoved?.Invoke(this, e);
        }

        #endregion

        #region Service Registration

        /// <inheritdoc />
        public IUnregister AddService<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            AddServiceInternal(typeof(T), service);
            return new CustomUnregister(() => RemoveService<T>());
        }

        /// <inheritdoc />
        public IUnregister AddService(Type type, object service)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (service == null) throw new ArgumentNullException(nameof(service));

            AddServiceInternal(type, service);
            return new CustomUnregister(() => RemoveService(type));
        }

        /// <summary>
        /// Core implementation for adding a service to the registry.
        /// </summary>
        /// <param name="type">The type to associate with the service.</param>
        /// <param name="service">The service instance to register.</param>
        /// <exception cref="ArgumentException">Thrown if the service does not implement the type or is already registered.</exception>
        private void AddServiceInternal(Type type, object service)
        {
            if (!type.IsAssignableFrom(service.GetType()))
                throw new ArgumentException($"The service of type {service.GetType().Name} does not implement or inherit from {type.Name}");

            lock (_services)
            {
                if (_services.ContainsKey(type))
                    throw new ArgumentException($"A service is already registered for type {type.Name}", nameof(type));

                _services.Add(type, service);
            }

            OnServiceAdded(new ServiceEventArgs(type, service));
        }

        #endregion

        #region Service Retrieval

        /// <inheritdoc />
        public T GetService<T>() where T : class
        {
            return (T)GetService(typeof(T));
        }

        /// <inheritdoc />
        public object GetService(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            lock (_services)
            {
                if (_services.TryGetValue(type, out var service))
                    return service;
            }

            return null;
        }

        /// <inheritdoc />
        public bool TryGetService<T>(out T service) where T : class
        {
            var type = typeof(T);
            lock (_services)
            {
                if (_services.TryGetValue(type, out var obj) && obj is T typedService)
                {
                    service = typedService;
                    return true;
                }
            }

            service = null;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetService(Type type, out object service)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            lock (_services)
            {
                if (_services.TryGetValue(type, out service))
                {
                    return true;
                }
            }

            service = null;
            return false;
        }

        #endregion

        #region Service Removal

        /// <inheritdoc />
        public void RemoveService<T>() where T : class
        {
            RemoveService(typeof(T));
        }

        /// <inheritdoc />
        public void RemoveService(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            object removedService = null;
            lock (_services)
            {
                if (_services.TryGetValue(type, out removedService))
                    _services.Remove(type);
            }

            if (removedService != null)
                OnServiceRemoved(new ServiceEventArgs(type, removedService));
        }

        #endregion

        #region Reset and Cleanup

        /// <summary>
        /// Resets the service registry, removing all services and clearing all event subscriptions.
        /// </summary>
        internal virtual void ResetRegistry()
        {
            ServiceAdded = null;
            ServiceRemoved = null;
            _services.Clear();
        }

        #endregion
    }


    #region Unity-Specific Service Registry

    /// <summary>
    /// Unity-specific service registry that manages MonoBehaviour services with their GameObjects.
    /// </summary>
    /// <remarks>
    /// The <see cref="PiServiceRegistry"/> is a specialized implementation of <see cref="ServiceRegistry"/> 
    /// designed for Unity-based applications. It provides additional functionality for managing 
    /// MonoBehaviour services by associating them with GameObjects and organizing them under a dedicated
    /// container GameObject that persists across scene loads.
    /// 
    /// This class follows the singleton pattern, and its instance can be accessed via the <see cref="Instance"/> property.
    /// </remarks>
    public class PiServiceRegistry : ServiceRegistry
    {
        #region Fields and Properties

        /// <summary>
        /// Singleton instance of the service registry.
        /// </summary>
        private static PiServiceRegistry _instance;

        /// <summary>
        /// Singleton instance of the service registry.
        /// </summary>
        internal static PiServiceRegistry Instance
        {
            get
            {
                _instance ??= new PiServiceRegistry();
                return _instance;
            }
        }

        /// <summary>
        /// GameObject associated with each service (for MonoBehaviour services).
        /// </summary>
        private readonly Dictionary<Type, GameObject> _serviceGameObjects;

        /// <summary>
        /// Container GameObject for all service GameObjects.
        /// </summary>
        internal GameObject ServiceContainer { get; private set; }

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Initializes a new instance of the Unity-specific service registry.
        /// </summary>
        internal PiServiceRegistry()
        {
            _serviceGameObjects = new Dictionary<Type, GameObject>();
            CreateServiceContainer();
        }

        /// <summary>
        /// Creates the container GameObject for services.
        /// </summary>
        private void CreateServiceContainer()
        {
            ServiceContainer = new GameObject("Services");
            GameObject.DontDestroyOnLoad(ServiceContainer);
        }

        #endregion

        #region MonoBehaviour Service Management

        /// <summary>
        /// Adds a MonoBehaviour service and attaches its GameObject to the service container.
        /// </summary>
        /// <param name="type">The type to associate with the service.</param>
        /// <param name="service">The MonoBehaviour service instance.</param>
        /// <returns>An unregister token for later removal of the service.</returns>
        /// <exception cref="ArgumentException">Thrown if the service is not a MonoBehaviour or has no GameObject.</exception>
        public IUnregister AddServiceAndGameObject(Type type, object service)
        {
            var unregister = AddService(type, service);
            var mono = service as MonoBehaviour;

            if (!typeof(MonoBehaviour).IsAssignableFrom(service.GetType()) || mono == null || mono.gameObject == null)
                throw new ArgumentException("The service must be attached to a GameObject to be included!");

            mono.transform.SetParent(ServiceContainer.transform);
            _serviceGameObjects[type] = mono.gameObject;

            return unregister;
        }

        /// <summary>
        /// Adds a MonoBehaviour service and attaches its GameObject to the service container.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The MonoBehaviour service instance.</param>
        /// <returns>An unregister token for later removal of the service.</returns>
        /// <exception cref="ArgumentException">Thrown if the service is not a MonoBehaviour or has no GameObject.</exception>
        public IUnregister AddServiceAndGameObject<T>(T service) where T : MonoBehaviour
        {
            return AddServiceAndGameObject(typeof(T), service);
        }

        #endregion

        #region Event Overrides

        /// <summary>
        /// Raises the ServiceRemoved event and cleans up associated GameObjects.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnServiceRemoved(ServiceEventArgs e)
        {
            base.OnServiceRemoved(e);
            if (_serviceGameObjects.TryGetValue(e.ServiceType, out GameObject go))
            {
                _serviceGameObjects.Remove(e.ServiceType);
                GameObject.Destroy(go);
            }
        }

        #endregion

        #region Reset and Cleanup

        /// <summary>
        /// Resets the service registry, removing all services, clearing event subscriptions,
        /// and recreating the service container.
        /// </summary>
        internal override void ResetRegistry()
        {
            base.ResetRegistry();
            _serviceGameObjects.Clear();

            if (ServiceContainer)
            {
                ServiceContainer.SetActive(false);
                GameObject.Destroy(ServiceContainer);
            }

            CreateServiceContainer();
        }

        #endregion
    }

    #endregion
}