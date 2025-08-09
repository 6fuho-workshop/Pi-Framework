using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Common
{
    /// <summary>
    /// Defines a generic interface for object factories that can create instances of type T.
    /// </summary>
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// Creates and returns a new instance of type T.
        /// </summary>
        T Create();
    }

    /// <summary>
    /// Default implementation of IObjectFactory that creates new instances using the default constructor.
    /// </summary>
    /// <typeparam name="T">The type of object to create. Must have a parameterless constructor.</typeparam>
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        /// <summary>
        /// Creates a new instance of T using its default constructor.
        /// </summary>
        public T Create()
        {
            return new T();
        }
    }

    /// <summary>
    /// Custom implementation of IObjectFactory that uses a provided factory method to create instances.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        /// <summary>
        /// Initializes the factory with a custom creation method.
        /// </summary>
        /// <param name="factoryMethod">A delegate that returns a new instance of T.</param>
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }

        /// <summary>
        /// The delegate used to create new instances of T.
        /// </summary>
        protected Func<T> mFactoryMethod;

        /// <summary>
        /// Creates a new instance of T using the provided factory method.
        /// </summary>
        public T Create()
        {
            return mFactoryMethod();
        }
    }

}