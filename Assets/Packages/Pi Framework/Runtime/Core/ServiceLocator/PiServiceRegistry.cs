using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PiFramework
{
    /// <summary>
    /// acts as a central registry that provides implementations of different interfaces. 
    /// By doing that, your component that uses an interface no longer needs to know the class that implements the interface. 
    /// Instead of instantiating that class itself, it gets an implementation from the service locator.
    /// </summary>
    public class PiServiceRegistry : IServiceRegistry
    {
        readonly Dictionary<Type, object> services;

        /// <summary>
        /// Game Object tương ứng với mỗi service
        /// </summary>
        readonly Dictionary<Type, GameObject> serviceGoDict;

        private static PiServiceRegistry _instance;
        internal GameObject objectContainer;

        //The service locator itself might be a singleton.There usually is no need to have two instances of a service locator.
        internal PiServiceRegistry()
        {
            services = new();
            serviceGoDict = new();
            CreateObjectContainer();
        }

        void CreateObjectContainer()
        {
            objectContainer = new GameObject("Services");
            GameObject.DontDestroyOnLoad(objectContainer);
        }

        internal static PiServiceRegistry instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PiServiceRegistry();
                }
                return _instance;
            }
        }

        /// <inheritdoc />
        public event EventHandler<ServiceEventArgs> ServiceAdded;

        /// <inheritdoc />
        public event EventHandler<ServiceEventArgs> ServiceRemoved;


        /// <inheritdoc />
        public T GetService<T>() where T : class
        {
            var type = typeof(T);
            lock (services)
            {
                if (services.TryGetValue(type, out var service))
                    return (T)service;
            }

            return null;
        }

        /// <inheritdoc />
        /// <remarks>
        /// This implementation triggers the <see cref="ServiceRemoved"/> event after a service is successfully removed.
        /// If the service type is not found, this method does nothing.
        /// </remarks>
        public void RemoveService<T>() where T : class
        {
            var type = typeof(T);
            object oldService;
            lock (services)
            {
                if (services.TryGetValue(type, out oldService))
                {
                    services.Remove(type);
                    if(serviceGoDict.TryGetValue(type, out GameObject go))
                    {
                        serviceGoDict.Remove(type);
                        GameObject.Destroy(go);
                    }
                }
            }
            if (oldService != null)
                OnServiceRemoved(new ServiceEventArgs(type, oldService));
        }

        private void OnServiceAdded(ServiceEventArgs e)
        {
            ServiceAdded?.Invoke(this, e);
        }

        private void OnServiceRemoved(ServiceEventArgs e)
        {
            ServiceRemoved?.Invoke(this, e);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This implementation triggers the <see cref="ServiceAdded"/> event after a service is successfully added.
        /// </remarks>
        public IUnRegister AddService<T>(T service) where T : class
        {
            AddService(typeof(T), service);
            return new CustomUnRegister(() => RemoveService<T>());
        }

        /// <summary>
        /// Thêm hoặc replace service nếu đã tồn tại
        /// </summary>
        /// <param name="type">service type, can be interface...</param>
        /// <param name="provider">service</param>
        internal void AddService(Type type, object service, bool includeGameObject = false)
        {
            AddService(type, service);

            if (includeGameObject)
            {
                if (!typeof(MonoBehaviour).IsAssignableFrom(type) || (service as MonoBehaviour).gameObject == null)
                    throw new ArgumentException("The provider does not attached to GameObject!");
                var transform = (service as MonoBehaviour).transform;
                transform.SetParent(objectContainer.transform);
                serviceGoDict[type] = transform.gameObject;
            }
        }

        void AddService(Type type, object service)
        {
            if (!type.IsAssignableFrom(service.GetType()))
                throw new ArgumentException("The provider does not match the specified service type!");

            if (service == null) throw new ArgumentNullException(nameof(service));

            lock (services)
            {
                if (services.ContainsKey(type))
                    throw new ArgumentException("Service is already registered with this type", nameof(type));
                services.Add(type, service);
            }
            OnServiceAdded(new ServiceEventArgs(type, service));
        }


        internal void Reset()
        {
            ServiceAdded = null;
            ServiceRemoved = null;
            services.Clear();
            serviceGoDict.Clear();
            if (objectContainer)
            {
                objectContainer.SetActive(false);
                GameObject.Destroy(objectContainer);
            }
            CreateObjectContainer();
        }
    }
}