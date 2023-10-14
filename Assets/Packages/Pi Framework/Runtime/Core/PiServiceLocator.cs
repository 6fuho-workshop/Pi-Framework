using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    /// <summary>
    /// acts as a central registry that provides implementations of different interfaces. 
    /// By doing that, your component that uses an interface no longer needs to know the class that implements the interface. 
    /// Instead of instantiating that class itself, it gets an implementation from the service locator.
    /// </summary>
    public class PiServiceLocator : IServiceProvider
    {
        readonly Dictionary<Type, object> _serviceDict = new Dictionary<Type, object>();

        /// <summary>
        /// Game Object tương ứng với mỗi service
        /// </summary>
        readonly Dictionary<Type, GameObject> _serviceGoDict = new Dictionary<Type, GameObject>();

        private static PiServiceLocator _instance = null;
        internal GameObject container;

        //The service locator itself might be a singleton.There usually is no need to have two instances of a service locator.
        internal PiServiceLocator()
        {
            container = new GameObject("Services");
            GameObject.DontDestroyOnLoad(container);
        }

        public static PiServiceLocator instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PiServiceLocator();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Thêm hoặc replace service nếu đã tồn tại
        /// </summary>
        /// <param name="type">service type, can be interface...</param>
        /// <param name="provider">service</param>
        public void AddService(object provider, Type type = null, bool includeGameObject = false)
        {
            AddService(false, provider, type, includeGameObject);
        }

        internal void AddModule(object provider)
        {
            AddService(true, provider, provider.GetType(), false);
        }

        /// <summary>
        /// Thêm hoặc replace service nếu đã tồn tại
        /// </summary>
        /// <param name="type">service type, can be interface...</param>
        /// <param name="provider">service</param>
        internal void AddService(bool isModule, object provider, Type type = null, bool includeGameObject = false)
        {

            if (provider == null)
                throw new ArgumentNullException("provider");
            if (type == null)
                type = provider.GetType();
            if (!type.IsAssignableFrom(provider.GetType()))
                throw new ArgumentException("The provider does not match the specified service type!");
            if (!isModule && typeof(PiModule).IsAssignableFrom(type))
            {
                AddModule(provider);
                return;
            }

            if (_serviceDict.ContainsKey(type))
            {
                if (isModule)
                {
                    Debug.LogError("Module of type " + type.ToString() + " already existed");
                }
                else
                {
                    GameObject.Destroy(_serviceGoDict[type]);
                }
            }
            _serviceDict[type] = provider;

            if (includeGameObject)
            {
                if (!typeof(MonoBehaviour).IsAssignableFrom(type) || (provider as MonoBehaviour).gameObject == null)
                    throw new ArgumentException("The provider does not attached to GameObject!");
                var transform = (provider as MonoBehaviour).transform;
                transform.SetParent(container.transform);
                _serviceGoDict[type] = transform.gameObject;
            }
            else if (!isModule)
            {
                var gO = new GameObject(type.Name);
                gO.transform.SetParent(container.transform);
                _serviceGoDict[type] = gO;
            }
        }

        public void AddService<T>(object provider, bool includeGameObject = false)
        {
            AddService(provider, typeof(T), includeGameObject);
        }

        public object GetService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            //object service;
            if (_serviceDict.TryGetValue(type, out object service))
                return service;
            throw new Exception("Service type " + type.ToString() + " not found");
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public void RemoveService(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            _serviceDict.Remove(type);
            _serviceGoDict.Remove(type);
        }

        internal void Reset()
        {
            _serviceDict.Clear();
            _serviceGoDict.Clear();
            if (container)
            {
                container.SetActive(false);
                GameObject.Destroy(container);
            }
            container = new GameObject("Services");
            GameObject.DontDestroyOnLoad(container);
        }
    }
}