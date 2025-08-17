using PF.Core.Common;
using PF.Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PF.Core.Services
{
    /// <summary>
    /// Internal implementation of a service registry, supporting registration, resolution, replacement, and removal of services and factories.
    /// Thread-safe via locking. Raises events on registration and unregistration.
    /// </summary>
    internal class ServiceRegistry : IServiceRegistry
    {
        #region Fields & Events

        /// <summary>
        /// Stores service providers (either instance or factory) by service type.
        /// </summary>
        protected readonly Dictionary<Type, IServiceProvider> _providers = new();
        /// <summary>
        /// Lock object for thread safety.
        /// </summary>
        protected readonly object _lock = new();

        /// <summary>
        /// Event raised when a service is registered.
        /// </summary>
        public event EventHandler<ServiceRegisteredEventArgs> ServiceRegistered;
        /// <summary>
        /// Event raised when a service is unregistered.
        /// </summary>
        public event EventHandler<ServiceUnregisteredEventArgs> ServiceUnregistered;

        /// <summary>
        /// Invokes the ServiceRegistered event.
        /// </summary>
        protected virtual void OnServiceRegistered(ServiceRegisteredEventArgs e)
            => ServiceRegistered?.Invoke(this, e);

        /// <summary>
        /// Invokes the ServiceUnregistered event.
        /// </summary>
        protected virtual void OnServiceUnregistered(ServiceUnregisteredEventArgs e)
            => ServiceUnregistered?.Invoke(this, e);

        #endregion

        #region Factory Registration

        /// <summary>
        /// Registers a factory for type T that does not require the registry context.
        /// </summary>
        public IUnregister RegisterFactory<T>(Func<T> factory) where T : class
            => RegisterFactory(typeof(T), _ => factory());

        /// <summary>
        /// Registers a factory for a specific type that does not require the registry context.
        /// </summary>
        public IUnregister RegisterFactory(Type serviceType, Func<object> factory)
            => RegisterFactory(serviceType, _ => factory());

        /// <summary>
        /// Registers a factory for type T that receives the registry context.
        /// </summary>
        public IUnregister RegisterFactory<T>(Func<IServiceRegistry, T> factory) where T : class
            => RegisterFactory(typeof(T), reg => factory(reg));

        /// <summary>
        /// Registers a factory for a specific type that receives the registry context.
        /// Throws if a provider is already registered for the type.
        /// </summary>
        public IUnregister RegisterFactory(Type serviceType, Func<IServiceRegistry, object> factory)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            lock (_lock)
            {
                if (_providers.ContainsKey(serviceType))
                    throw new ArgumentException($"Provider already registered for {serviceType.Name}");
                _providers.Add(serviceType, new FactoryProvider(factory));
            }
            return new CustomUnregister(() => RemoveFactory(serviceType));
        }

        /// <summary>
        /// Attempts to register a factory for type T. Returns false if already registered.
        /// </summary>
        public bool TryRegisterFactory<T>(Func<T> factory) where T : class
            => TryRegisterFactory(typeof(T), _ => factory());

        /// <summary>
        /// Attempts to register a factory for a specific type. Returns false if already registered.
        /// </summary>
        public bool TryRegisterFactory(Type serviceType, Func<object> factory)
            => TryRegisterFactory(serviceType, _ => factory());

        /// <summary>
        /// Attempts to register a factory for a specific type with registry context. Returns false if already registered.
        /// </summary>
        private bool TryRegisterFactory(Type serviceType, Func<IServiceRegistry, object> factory)
        {
            if (serviceType == null || factory == null) return false;
            lock (_lock)
            {
                if (_providers.ContainsKey(serviceType)) return false;
                _providers.Add(serviceType, new FactoryProvider(factory));
                return true;
            }
        }

        /// <summary>
        /// Removes the factory for type T if it exists.
        /// </summary>
        public bool RemoveFactory<T>() where T : class => RemoveFactory(typeof(T));

        /// <summary>
        /// Removes the factory for a specific type if it exists.
        /// </summary>
        public bool RemoveFactory(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                if (_providers.TryGetValue(serviceType, out var provider) && !provider.IsInstance)
                {
                    return _providers.Remove(serviceType);
                }
                return false;
            }
        }

        #endregion

        #region Service Registration

        /// <summary>
        /// Registers a service instance of type T.
        /// </summary>
        public IUnregister Register<T>(T instance) where T : class
            => Register(typeof(T), instance);

        /// <summary>
        /// Registers a service instance for a specific type.
        /// Throws if a service is already registered for the type.
        /// </summary>
        public IUnregister Register(Type serviceType, object instance)
        {
            ThrowIf.Null(serviceType);
            ThrowIf.Null(instance);
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"Instance {instance.GetType().Name} does not implement {serviceType.Name}");

            lock (_lock)
            {
                if (_providers.ContainsKey(serviceType))
                    throw new ArgumentException($"Service already registered for {serviceType.Name}", nameof(serviceType));
                _providers.Add(serviceType, new InstanceProvider(instance));
            }

            OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
            return new ServiceUnregister(this, serviceType);
        }

        /// <summary>
        /// Attempts to register a service instance of type T. Returns false if already registered.
        /// </summary>
        public bool TryRegister<T>(T instance) where T : class
            => TryRegister(typeof(T), instance);

        /// <summary>
        /// Attempts to register a service instance for a specific type. Returns false if already registered.
        /// </summary>
        public bool TryRegister(Type serviceType, object instance)
        {
            if (serviceType == null) return false;
            if (instance == null) return false;
            if (!serviceType.IsAssignableFrom(instance.GetType())) return false;

            bool added = false;
            lock (_lock)
            {
                if (!_providers.ContainsKey(serviceType))
                {
                    _providers.Add(serviceType, new InstanceProvider(instance));
                    added = true;
                }
            }
            if (added)
                OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
            return added;
        }

        /// <summary>
        /// Replaces the existing service of type T, or registers it if not present.
        /// Optionally disposes the old instance.
        /// </summary>
        public void Replace<T>(T instance, bool disposeOld = true) where T : class
            => Replace(typeof(T), instance, disposeOld);

        /// <summary>
        /// Replaces the existing service for a specific type, or registers it if not present.
        /// Optionally disposes the old instance.
        /// </summary>
        public void Replace(Type serviceType, object instance, bool disposeOld = true)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"Instance {instance.GetType().Name} does not implement {serviceType.Name}");

            object old = null;
            lock (_lock)
            {
                if (_providers.TryGetValue(serviceType, out var oldProvider))
                {
                    old = oldProvider.Instance; // Will be null for factory providers
                    _providers[serviceType] = new InstanceProvider(instance);
                }
                else
                {
                    _providers.Add(serviceType, new InstanceProvider(instance));
                }
            }
            if (old != null)
            {
                OnServiceUnregistered(new ServiceUnregisteredEventArgs(serviceType, old));
                if (disposeOld)
                    DisposeService(old);
            }
            OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
        }

        /// <summary>
        /// Removes the service of type T if it exists. Optionally disposes the instance.
        /// </summary>
        public bool Remove<T>(bool disposeInstance = true) where T : class
            => Remove(typeof(T), disposeInstance);

        /// <summary>
        /// Removes the service for a specific type if it exists. Optionally disposes the instance.
        /// </summary>
        public bool Remove(Type serviceType, bool disposeInstance = true)
        {
            if (serviceType == null) return false;
            object removed = null;
            lock (_lock)
            {
                if (_providers.TryGetValue(serviceType, out var provider))
                {
                    removed = provider.Instance; // Will be null for factory providers
                    _providers.Remove(serviceType);
                }
            }
            if (removed != null)
            {
                OnServiceUnregistered(new ServiceUnregisteredEventArgs(serviceType, removed));
                if (disposeInstance)
                {
                    DisposeService(removed);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disposes the given service instance if it implements <see cref="IDisposable"/>.
        /// Any exceptions thrown during disposal are caught and logged as errors.
        /// </summary>
        /// <param name="service">The service instance to dispose.</param>
        private void DisposeService(object service)
        {
            if (service is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during disposal to the Unity console.
                    UnityEngine.Debug.LogError($"Error disposing service: {ex}");
                }
            }
        }

        /// <summary>
        /// Removes all registered services and factories.
        /// Optionally disposes instances that implement <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="dispose">If true, disposes all service instances.</param>
        public virtual void Clear(bool dispose = true)
        {
            Clear(dispose, true);
        }

        /// <summary>
        /// Services will be cleared and disposed in LIFO (Last-In, First-Out) order.
        /// If <paramref name="disposeInstances"/> is true, disposes all service instances implementing <see cref="IDisposable"/>.
        /// If <paramref name="removeFactories"/> is true, removes all registered factories.
        /// </summary>
        public virtual void Clear(bool disposeInstances, bool removeFactories)
        {
            List<KeyValuePair<Type, IServiceProvider>> snapshot;

            lock (_lock)
            {
                snapshot = new List<KeyValuePair<Type, IServiceProvider>>(_providers);
                _providers.Clear();
            }

            // Đảo ngược thứ tự để đảm bảo LIFO dispose
            for (int i = snapshot.Count - 1; i >= 0; i--)
            {
                var kv = snapshot[i];
                var provider = kv.Value;

                if (provider.IsInstance)
                {
                    var instance = provider.Instance;
                    OnServiceUnregistered(new ServiceUnregisteredEventArgs(kv.Key, instance));
                    if (disposeInstances && instance is IDisposable d) d.Dispose();
                }
                else if (!removeFactories)
                {
                    // Re-add factory if we're keeping them
                    lock (_lock)
                    {
                        _providers.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        #endregion

        #region Service Resolution

        /// <summary>
        /// Resolves and returns an instance of type T.
        /// Throws if the service is not registered.
        /// </summary>
        public T Resolve<T>() where T : class => (T)Resolve(typeof(T));

        /// <summary>
        /// Resolves and returns an instance for a specific type.
        /// Throws if the service is not registered.
        /// </summary>
        public object Resolve(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (TryResolve(serviceType, out var obj)) return obj;
            throw new InvalidOperationException($"Service {serviceType.Name} not found.");
        }

        /// <summary>
        /// Attempts to resolve an instance of type T. Returns true if found.
        /// </summary>
        public bool TryResolve<T>(out T instance) where T : class
        {
            if (TryResolve(typeof(T), out var obj) && obj is T cast)
            {
                instance = cast;
                return true;
            }
            instance = null;
            return false;
        }

        /// <summary>
        /// Attempts to resolve an instance for a specific type. Returns true if found.
        /// Handles both instance and factory providers, and caches factory results.
        /// </summary>
        public bool TryResolve(Type serviceType, out object instance)
        {
            if (serviceType == null) { instance = null; return false; }

            IServiceProvider provider = null;
            bool isFactory = false;

            // 1) Lock ngắn: tìm provider (không gọi GetService trong lock)
            lock (_lock)
            {
                if (!_providers.TryGetValue(serviceType, out provider))
                {
                    instance = null;
                    return false;
                }
                isFactory = !provider.IsInstance;
            }

            // 2) Nếu là instance provider, trả về instance ngay
            if (!isFactory)
            {
                instance = provider.GetService(this);
                return true;
            }

            // 3) Nếu là factory provider, tạo instance và cache nếu cần
            var created = provider.GetService(this);
            if (created == null || !serviceType.IsAssignableFrom(created.GetType()))
                throw new InvalidOperationException(
                    $"Factory produced {created?.GetType().Name ?? "null"} not assignable to {serviceType.Name}");

            bool createdByUs = false;

            // Double-check và cache
            lock (_lock)
            {
                // Kiểm tra lại xem đã có instance chưa
                if (_providers.TryGetValue(serviceType, out var currentProvider) && currentProvider.IsInstance)
                {
                    instance = currentProvider.GetService(this); // Có thread khác đã tạo trước
                }
                else
                {
                    // Cache instance mới tạo
                    var instanceProvider = new InstanceProvider(created);
                    _providers[serviceType] = instanceProvider;
                    instance = created;
                    createdByUs = true;
                }
            }

            // 4) Bắn event ngoài lock nếu chúng ta đã cache instance mới
            if (createdByUs)
                OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, created));

            return true;
        }

        /// <summary>
        /// Ensures that an instance of type T is created and registered.
        /// </summary>
        public T Ensure<T>() where T : class => (T)Ensure(typeof(T));

        /// <summary>
        /// Ensures that an instance for a specific type is created and registered.
        /// </summary>
        public object Ensure(Type serviceType)
        {
            return Resolve(serviceType);
        }

        /// <summary>
        /// Checks if a service of type T is registered (instance or factory).
        /// </summary>
        public bool Contains<T>() where T : class => Contains(typeof(T));

        /// <summary>
        /// Checks if a service for a specific type is registered (instance or factory).
        /// </summary>
        public bool Contains(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                return _providers.ContainsKey(serviceType);
            }
        }

        #endregion

        #region IReadOnlyServiceLocator Extensions

        /// <summary>
        /// Checks if an instance of type T is registered (not factory).
        /// </summary>
        public bool HasInstance<T>() where T : class => HasInstance(typeof(T));

        /// <summary>
        /// Checks if an instance for a specific type is registered (not factory).
        /// </summary>
        public bool HasInstance(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                return _providers.TryGetValue(serviceType, out var provider) && provider.IsInstance;
            }
        }

        /// <summary>
        /// Checks if a factory for type T is registered (not instance).
        /// </summary>
        public bool HasFactory<T>() where T : class => HasFactory(typeof(T));

        /// <summary>
        /// Checks if a factory for a specific type is registered (not instance).
        /// </summary>
        public bool HasFactory(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                return _providers.TryGetValue(serviceType, out var provider) && !provider.IsInstance;
            }
        }

        /// <summary>
        /// Gets all registered service types (instances and/or factories).
        /// </summary>
        public IEnumerable<Type> GetRegisteredServiceTypes()
        {
            lock (_lock)
            {
                return _providers.Keys.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Helper class for unregistering a service.
        /// </summary>
        private sealed class ServiceUnregister : IUnregister
        {
            private readonly IServiceRegistry _owner;
            private readonly Type _type;

            public ServiceUnregister(IServiceRegistry owner, Type type)
            {
                _owner = owner;
                _type = type;
                IsRegistered = true;
            }

            public bool IsRegistered { get; private set; }

            /// <summary>
            /// Unregisters the service if it is still registered.
            /// </summary>
            public void Unregister()
            {
                if (!IsRegistered)
                    return;
                IsRegistered = false;
                _owner.Remove(_type);
            }
        }
    }
}