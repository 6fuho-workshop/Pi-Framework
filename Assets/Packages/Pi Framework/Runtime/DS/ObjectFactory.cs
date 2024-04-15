using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.DS
{
    public interface IObjectFactory<T>
    {
        T Create();
    }


    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public T Create()
        {
            return new T();
        }
    }


    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            mFactoryMethod = factoryMethod;
        }

        protected Func<T> mFactoryMethod;

        public T Create()
        {
            return mFactoryMethod();
        }
    }

}