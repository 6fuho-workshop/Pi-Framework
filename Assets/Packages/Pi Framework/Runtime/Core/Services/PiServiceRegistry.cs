// PiServiceRegistry.cs (refactored)
// Namespace giữ nguyên để tối thiểu hoá phạm vi thay đổi ngoài file này.
using PF.Core.Common;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace PF.Core.Services.Unity
{
    /// <summary>
    /// Unity-specific wrapper for ServiceRegistry.
    /// - Gắn các MonoBehaviour services vào một container GameObject (DontDestroyOnLoad).
    /// - Hỗ trợ đăng ký factory tạo MonoBehaviour và tự gắn vào container.
    /// - Không singleton. Hãy khởi tạo và giữ tham chiếu tại PFApp/PFContext.
    /// </summary>
    public sealed class PiServiceRegistry : ServiceRegistry
    {
        internal static PiServiceRegistry Instance
        {
            get
            {
                _instance ??= new PiServiceRegistry();
                return _instance;
            }
        }
        private static PiServiceRegistry _instance;

        private readonly Dictionary<Type, GameObject> _serviceGameObjects = new();
        public GameObject ServiceContainer { get; private set; }

        public PiServiceRegistry(string containerName = "Services")
        {
            CreateServiceContainer(containerName);
        }

        private void CreateServiceContainer(string name)
        {
            ServiceContainer = new GameObject(name);
            GameObject.DontDestroyOnLoad(ServiceContainer);
        }

        /// <summary>
        /// Đăng ký một service là MonoBehaviour và gắn GameObject của nó vào container.
        /// - Throw nếu instance không phải MonoBehaviour hợp lệ.
        /// </summary>
        public IUnregister RegisterWithGO(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (instance is not MonoBehaviour)
                throw new ArgumentException($"{serviceType} must be a MonoBehaviour to register with a GameObject.");

            var mono = instance as MonoBehaviour;
            if (mono == null || mono.gameObject == null)
                throw new ArgumentException("Instance must be a MonoBehaviour bound to a valid GameObject.");

            // Đăng ký vào registry base (sẽ bắn ServiceRegistered)
            var token = Register(serviceType, instance);

            // Gắn vào container
            mono.transform.SetParent(ServiceContainer.transform, worldPositionStays: false);
            _serviceGameObjects[serviceType] = mono.gameObject;

            return token;
        }

        public IUnregister RegisterWithGO<T>(T instance) where T : MonoBehaviour
            => RegisterWithGO(typeof(T), instance);

        /// <summary>
        /// Đăng ký factory tạo MonoBehaviour; khi Resolve lần đầu sẽ tạo GO và gắn vào container.
        /// </summary>
        public IUnregister RegisterFactoryWithGO<T>(Func<GameObject, T> factory) where T : MonoBehaviour
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return RegisterFactory(typeof(T), reg => CreateMonoAndTrack(factory));
        }

        private T CreateMonoAndTrack<T>(Func<GameObject, T> factory) where T : MonoBehaviour
        {
            GameObject go = null;
            try
            {
                go = new GameObject(typeof(T).Name);
                go.transform.SetParent(ServiceContainer.transform, worldPositionStays: false);

                var instance = factory(go);                 // tạo ngoài lock
                if (instance == null || instance.gameObject == null)
                    throw new InvalidOperationException($"Factory must attach a valid {typeof(T).Name} to a GameObject.");

                // Nếu factory trả về component trên GO khác, huỷ GO tạm để tránh rác
                if (instance.gameObject != go && go != null)
                    UnityEngine.Object.Destroy(go);

                _serviceGameObjects[typeof(T)] = instance.gameObject;
                return instance;
            }
            catch
            {
                if (go != null)
                    UnityEngine.Object.Destroy(go);
                throw;
            }
        }

        /// <summary>
        /// Xoá service; Unity side sẽ dọn GameObject tương ứng.
        /// (Base class sẽ bắn event Unregistered, mình hook ở đây để destroy GO.)
        /// </summary>
        protected override void OnServiceUnregistered(ServiceUnregisteredEventArgs e)
        {
            base.OnServiceUnregistered(e);

            if (_serviceGameObjects.TryGetValue(e.ServiceType, out var go))
            {
                _serviceGameObjects.Remove(e.ServiceType);
                if (go) GameObject.Destroy(go);
            }
        }

        public override void Clear(bool disposeInstances = true, bool removeFactories = true)
        {
            // Dọn phần registry (bắn Unregistered + dispose nếu có)
            base.Clear(disposeInstances, removeFactories);

            // Dọn GO
            foreach (var kv in _serviceGameObjects)
            {
                var go = kv.Value;
                if (go) GameObject.Destroy(go);
            }
            _serviceGameObjects.Clear();

            if (ServiceContainer)
            {
                ServiceContainer.SetActive(false);
                GameObject.Destroy(ServiceContainer);
                ServiceContainer = null;
            }

            
            CreateServiceContainer("Services");
        }
    }
}
