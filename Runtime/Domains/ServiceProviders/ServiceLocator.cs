using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeadWindCSS.Domains.ServiceProviders
{
    /// <summary>
    /// Base interface for our service locator to work with. Services implementing
    /// this interface will be retrievable using the locator.
    /// </summary> 
    public interface IService
    {
        public void Initialize();
    }
    
    /// <summary>
    /// Simple service locator for <see cref="IService"/> instances.
    /// </summary>
    public class ServiceLocator
    {
        private ServiceLocator() { }

        /// <summary>
        /// currently registered services.
        /// </summary>
        private readonly Dictionary<string, IService> _services = new ();

        /// <summary>
        /// Gets the currently active service locator instance.
        /// </summary>
        public static ServiceLocator Current { get; private set; }

        /// <summary>
        /// Initalizes the service locator with a new instance.
        /// </summary>
        public static void Initialize()
        {
            Current = new ServiceLocator();
        }

        /// <summary>
        /// Gets the service instance of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to lookup.</typeparam>
        /// <returns>The service instance.</returns>
        public T Get<T>() where T : IService
        {
            string key = typeof(T).Name;
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"{key} not registered with {GetType().Name}");
                throw new InvalidOperationException();
            }

            return (T)_services[key];
        }

        /// <summary>
        /// Registers the service with the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="service">Service instance.</param>
        public void Register<T>(T service) where T : IService
        {
            string key = typeof(T).Name;
            if (!_services.TryAdd(key, service))
            {
                Debug.LogError($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
                return;
            }
            
            service.Initialize();
        }

        /// <summary>
        /// Unregisters the service from the current service locator.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        public void Unregister<T>() where T : IService
        {
            string key = typeof(T).Name;
            if (!_services.ContainsKey(key))
            {
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
                return;
            }

            _services.Remove(key);
        }
    }
}