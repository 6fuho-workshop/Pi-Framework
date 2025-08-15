using System;

namespace PF.Core.Services
{
    /// <summary>
    /// Abstraction for service providers that can either return an existing instance or create one on demand
    /// </summary>
    internal interface IServiceProvider
    {
        /// <summary>
        /// Returns the service instance, either existing or created on demand
        /// </summary>
        object GetService(IServiceRegistry registry);
        
        /// <summary>
        /// Whether this provider holds an existing instance
        /// </summary>
        bool IsInstance { get; }
        
        /// <summary>
        /// Returns the held instance or null if this is a factory provider
        /// </summary>
        object Instance { get; }
    }

    /// <summary>
    /// Provider that holds a pre-created service instance
    /// </summary>
    internal class InstanceProvider : IServiceProvider
    {
        private readonly object _instance;

        public InstanceProvider(object instance)
        {
            _instance = instance;
        }

        public object GetService(IServiceRegistry registry) => _instance;
        public bool IsInstance => true;
        public object Instance => _instance;
    }

    /// <summary>
    /// Provider that creates service instances on demand using a factory
    /// </summary>
    internal class FactoryProvider : IServiceProvider
    {
        private readonly Func<IServiceRegistry, object> _factory;

        public FactoryProvider(Func<IServiceRegistry, object> factory)
        {
            _factory = factory;
        }

        public object GetService(IServiceRegistry registry) => _factory(registry);
        public bool IsInstance => false;
        public object Instance => null;
    }
}