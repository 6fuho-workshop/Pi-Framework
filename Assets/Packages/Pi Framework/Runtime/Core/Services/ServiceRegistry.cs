using PF.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PF.Core.Services
{
    #region Event Args

    public sealed class ServiceRegisteredEventArgs : EventArgs
    {
        public ServiceRegisteredEventArgs(Type serviceType, object instance)
        { ServiceType = serviceType; Instance = instance; }
        public Type ServiceType { get; }
        public object Instance { get; }
    }

    public sealed class ServiceUnregisteredEventArgs : EventArgs
    {
        public ServiceUnregisteredEventArgs(Type serviceType, object instance)
        { ServiceType = serviceType; Instance = instance; }
        public Type ServiceType { get; }
        public object Instance { get; }
    }

    #endregion

    public class ServiceRegistry : IServiceRegistry
    {
        #region Fields & Events

        protected readonly Dictionary<Type, object> _services = new();
        protected readonly Dictionary<Type, Func<IServiceRegistry, object>> _factories = new();
        protected readonly object _lock = new();

        public event EventHandler<ServiceRegisteredEventArgs> ServiceRegistered;
        public event EventHandler<ServiceUnregisteredEventArgs> ServiceUnregistered;

        protected virtual void OnServiceRegistered(ServiceRegisteredEventArgs e)
            => ServiceRegistered?.Invoke(this, e);

        protected virtual void OnServiceUnregistered(ServiceUnregisteredEventArgs e)
            => ServiceUnregistered?.Invoke(this, e);

        #endregion

        #region Factory Registration

        public IUnregister RegisterFactory<T>(Func<T> factory) where T : class
            => RegisterFactory(typeof(T), _ => factory());

        public IUnregister RegisterFactory(Type serviceType, Func<object> factory)
            => RegisterFactory(serviceType, _ => factory());

        public IUnregister RegisterFactory<T>(Func<IServiceRegistry, T> factory) where T : class
            => RegisterFactory(typeof(T), reg => factory(reg));

        public IUnregister RegisterFactory(Type serviceType, Func<IServiceRegistry, object> factory)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            lock (_lock)
            {
                if (_factories.ContainsKey(serviceType))
                    throw new ArgumentException($"Factory already registered for {serviceType.Name}");
                _factories.Add(serviceType, factory);
            }
            return new CustomUnregister(() => RemoveFactory(serviceType));
        }

        public bool TryRegisterFactory<T>(Func<T> factory) where T : class
            => TryRegisterFactory(typeof(T), _ => factory());

        public bool TryRegisterFactory(Type serviceType, Func<object> factory)
            => TryRegisterFactory(serviceType, _ => factory());

        private bool TryRegisterFactory(Type serviceType, Func<IServiceRegistry, object> factory)
        {
            if (serviceType == null || factory == null) return false;
            lock (_lock)
            {
                if (_factories.ContainsKey(serviceType)) return false;
                _factories.Add(serviceType, factory);
                return true;
            }
        }

        public bool RemoveFactory<T>() where T : class => RemoveFactory(typeof(T));

        public bool RemoveFactory(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock) return _factories.Remove(serviceType);
        }

        #endregion

        #region Service Registration

        public IUnregister Register<T>(T instance) where T : class
            => Register(typeof(T), instance);

        public IUnregister Register(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"Instance {instance.GetType().Name} does not implement {serviceType.Name}");

            lock (_lock)
            {
                if (_services.ContainsKey(serviceType))
                    throw new ArgumentException($"Service already registered for {serviceType.Name}", nameof(serviceType));
                _services.Add(serviceType, instance);
            }

            OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
            return new ServiceUnregister(this, serviceType);
        }

        public bool TryRegister<T>(T instance) where T : class
            => TryRegister(typeof(T), instance);

        public bool TryRegister(Type serviceType, object instance)
        {
            if (serviceType == null) return false;
            if (instance == null) return false;
            if (!serviceType.IsAssignableFrom(instance.GetType())) return false;

            bool added = false;
            lock (_lock)
            {
                if (!_services.ContainsKey(serviceType))
                {
                    _services.Add(serviceType, instance);
                    added = true;
                }
            }
            if (added)
                OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
            return added;
        }

        public void Replace<T>(T instance, bool disposeOld = true) where T : class
            => Replace(typeof(T), instance, disposeOld);

        public void Replace(Type serviceType, object instance, bool disposeOld = true)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"Instance {instance.GetType().Name} does not implement {serviceType.Name}");

            object old = null;
            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out old))
                    _services[serviceType] = instance;
                else
                    _services.Add(serviceType, instance);
            }
            if (old != null)
            {
                OnServiceUnregistered(new ServiceUnregisteredEventArgs(serviceType, old));
                if (disposeOld)
                    DisposeService(old);
            }
            OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, instance));
        }

        public bool Remove<T>(bool disposeInstance = true) where T : class
            => Remove(typeof(T), disposeInstance);

        public bool Remove(Type serviceType, bool disposeInstance = true)
        {
            if (serviceType == null) return false;
            object removed = null;
            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out removed))
                    _services.Remove(serviceType);
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
            List<KeyValuePair<Type, object>> snapshot;

            lock (_lock)
            {
                snapshot = new List<KeyValuePair<Type, object>>(_services);
                _services.Clear();
                if (removeFactories && _factories != null) _factories.Clear();
            }

            // Đảo ngược thứ tự để đảm bảo LIFO dispose
            for (int i = snapshot.Count - 1; i >= 0; i--)
            {
                var kv = snapshot[i];
                OnServiceUnregistered(new ServiceUnregisteredEventArgs(kv.Key, kv.Value));
                if (disposeInstances && kv.Value is IDisposable d) d.Dispose();
            }
        }

        #endregion

        #region Service Resolution

        public T Resolve<T>() where T : class => (T)Resolve(typeof(T));

        public object Resolve(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (TryResolve(serviceType, out var obj)) return obj;
            throw new InvalidOperationException($"Service {serviceType.Name} not found.");
        }

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

        public bool TryResolve(Type serviceType, out object instance)
        {
            if (serviceType == null) { instance = null; return false; }

            object existing;
            Func<IServiceRegistry, object> factory = null;

            // 1) Lock ngắn: chỉ đọc cache + lấy factory (không gọi factory)
            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out existing))
                {
                    instance = existing;
                    return true;
                }

                if (!_factories.TryGetValue(serviceType, out factory))
                {
                    instance = null;
                    return false;
                }
                // => Có factory, nhưng KHÔNG gọi trong lock
            }

            // 2) Gọi factory ngoài lock
            var created = factory(this);
            if (created == null || !serviceType.IsAssignableFrom(created.GetType()))
                throw new InvalidOperationException(
                    $"Factory produced {created?.GetType().Name ?? "null"} not assignable to {serviceType.Name}");

            bool createdByUs = false;

            // 3) Lock lần 2: double-check rồi cache nếu vẫn chưa có
            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out existing))
                {
                    instance = existing; // có thread khác đã tạo trước
                }
                else
                {
                    _services[serviceType] = created;
                    instance = created;
                    createdByUs = true;
                }
            }

            // 4) Bắn event ngoài lock
            if (createdByUs)
                OnServiceRegistered(new ServiceRegisteredEventArgs(serviceType, created));

            return true;
        }

        public T Ensure<T>() where T : class => (T)Ensure(typeof(T));

        public object Ensure(Type serviceType)
        {
            return Resolve(serviceType);
        }

        public bool Contains<T>() where T : class => Contains(typeof(T));

        public bool Contains(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                if (_services.ContainsKey(serviceType)) return true;
                if (_factories != null && _factories.ContainsKey(serviceType)) return true;
            }
            return false;
        }

        #endregion

        #region IReadOnlyServiceLocator Extensions

        public bool HasInstance<T>() where T : class => HasInstance(typeof(T));

        public bool HasInstance(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                return _services.ContainsKey(serviceType);
            }
        }

        public bool HasFactory<T>() where T : class => HasFactory(typeof(T));

        public bool HasFactory(Type serviceType)
        {
            if (serviceType == null) return false;
            lock (_lock)
            {
                return _factories.ContainsKey(serviceType);
            }
        }

        public IEnumerable<Type> GetRegisteredServiceTypes()
        {
            lock (_lock)
            {
                // Union of all registered instance types and factory types
                var all = new HashSet<Type>(_services.Keys);
                foreach (var t in _factories.Keys)
                    all.Add(t);
                return all.ToArray();
            }
        }

        #endregion


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