using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PiFramework.Pool
{
    public class PoolBuilder<T> where T : class
    {
        int _defaultCapacity = 10;
        int _maxSize = 10000;
        Func<T> _createFunc;
        Action<T> _actionOnGet;
        Action<T> _actionOnRelease;
        Action<T> _actionOnDestroy;
        bool _collectionCheck;

        public ObjectPool<T> Build()
        {
            return new ObjectPool<T>(_createFunc, _actionOnGet, _actionOnRelease, _actionOnDestroy, _collectionCheck, _defaultCapacity, _maxSize);
        }

        /// <summary>
        /// Set default capacity (default value: 10)
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public PoolBuilder<T> DefaultCapacity(int capacity)
        {
            _defaultCapacity = capacity;
            return this;
        }

        /// <summary>
        /// Set maxSize (default value: 10000)
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public PoolBuilder<T> MaxSize(int size)
        {
            _maxSize = size;
            return this;
        }

        /// <summary>
        /// Set Factory Method
        /// </summary>
        /// <param name="factoryMethod"></param>
        /// <returns></returns>
        public PoolBuilder<T> CreateFunc(Func<T> factoryMethod)
        {
            _createFunc = factoryMethod;
            return this;
        }

        /// <summary>
        /// Set Action on Get (Acquire object from pool)
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PoolBuilder<T> ActionOnGet(Action<T> action)
        {
            _actionOnGet = action;
            return this;
        }

        /// <summary>
        /// Set Action on Release object back to poool
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PoolBuilder<T> ActionOnRelease(Action<T> action)
        {
            _actionOnRelease = action;
            return this;
        }

        /// <summary>
        /// Set Action when object can not release back to pool and should be destroy
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PoolBuilder<T> ActionOnDestroy(Action<T> action)
        {
            _actionOnDestroy = action;
            return this;
        }

        /// <summary>
        /// enable collection check
        /// </summary>
        /// <returns></returns>
        public PoolBuilder<T> CollectionCheck()
        {
            _collectionCheck = true;
            return this;
        }
    }
}