using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Pool;

namespace PiFramework.Pool
{
    public class GameObjectPool : IDisposable, IObjectPool<GameObject>
    {
        ObjectPool<GameObject> _pool;
        int _preloadCount;

        public int CountInactive => _pool.CountInactive;


        public GameObjectPool(Func<GameObject> createFunc, Action<GameObject> actionOnRelease = null,
            int preloadCount = 0, int defaultCapacity = 10, int maxSize = 10000)
        {
            _pool = new ObjectPool<GameObject>(createFunc, null, actionOnRelease, null, false, defaultCapacity, maxSize);
            _preloadCount = preloadCount;
        }

        public void Preload()
        {
            if (_preloadCount <= 0)
                return;

        }

        public void Clear() => _pool.Clear();

        public void Dispose() => Clear();

        public GameObject Get()
        {
            return _pool.Get();
        }

        public PooledObject<GameObject> Get(out GameObject v)
        {
            return _pool.Get(out v);
        }

        public void Release(GameObject element)
        {
            _pool.Release(element);
        }
    }
}