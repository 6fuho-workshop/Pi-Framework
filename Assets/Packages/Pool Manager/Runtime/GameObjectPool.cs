using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PiFramework.Pool
{
    public class GameObjectPool : IObjectPool<GameObject>
    {
        const string poolPrefix = "[POOL]";
        static readonly Vector3 hidePos = Vector3.left * 100000;

        Func<GameObject> _creatFunc;
        Action<GameObject> _onGet;
        Action<GameObject> _onRelease;
        Action<GameObject> _onDestroy;
        ObjectPool<GameObject> _pool;

        public bool setParentOnCreate;
        public bool deactiveOnRelease;
        public bool hideOnRelease;
        public bool dontDestroyOnLoad;
        public int preloadCount;

        string _poolName;
        Transform _poolContainer;
        Transform poolContainer {
            get
            {
                if (_poolContainer == null)
                {
                    _poolContainer = new GameObject(poolPrefix + _poolName).transform;
                    if(dontDestroyOnLoad)
                        GameObject.DontDestroyOnLoad(_poolContainer);
                }
                return _poolContainer;
            }
        }

        public int CountInactive => _pool.CountInactive;
        public int CountActive => _pool.CountActive;
        public int CountAll => _pool.CountAll;

        internal int poolID {  get; set; }

        public GameObjectPool(Func<GameObject> createFunc, string poolName, int defaultCapacity = 10, int maxSize = 10000)
        {
            _creatFunc = createFunc;
            _pool = new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, false, defaultCapacity, maxSize);
            _poolName = poolName;
        }

        public GameObjectPool(GameObject origin, int defaultCapacity = 10, int maxSize = 10000)
            : this(() => { return GameObject.Instantiate(origin); }, origin.name, defaultCapacity, maxSize)
        {
        }

        internal GameObjectPool(Poolable poolable)
            : this(poolable.gameObject, poolable.defaultCapacity, poolable.maxSize)
        {
            preloadCount = poolable.preloadCount;
            setParentOnCreate = poolable.setParentOnCreate;
            deactiveOnRelease = poolable.deactiveOnRelease;
            hideOnRelease = poolable.hideOnRelease;
            dontDestroyOnLoad = poolable.dontDestroyOnLoad;
        }

        public void Preload()
        {
            var count = preloadCount;
            count -= _pool.CountActive;
            if (count <= 0)
                return;

            var tmpArray = new GameObject[count];

            for (int i = 0; i < count; i++)
                tmpArray[i] = _pool.Get();

            for (int i = 0; i < count; i++)
                _pool.Release(tmpArray[i]);
        }

        GameObjectPool SetActionOnGet(Action<GameObject> actionOnGet)
        {
            _onGet = actionOnGet;
            return this;
        }

        GameObjectPool SetActionOnRelease(Action<GameObject> actionOnRelease)
        {
            _onRelease = actionOnRelease;
            return this;
        }

        GameObjectPool SetActionOnDestroy(Action<GameObject> actionOnDestroy)
        {
            _onDestroy = actionOnDestroy;
            return this;
        }

        GameObject CreateFunc()
        {
            var go = _creatFunc();
            if (setParentOnCreate)
                go.transform.parent = poolContainer;

            if(poolID != 0)
                go.GetComponent<Poolable>().poolID = poolID;
            
            return go;
        }

        void ActionOnGet(GameObject obj)
        {
            _onGet?.Invoke(obj);
        }

        void ActionOnRelease(GameObject obj)
        {
            obj.transform.parent = poolContainer;
            
            _onRelease?.Invoke(obj);

            if (deactiveOnRelease)
                obj.SetActive(false);
            if(hideOnRelease)
                obj.transform.position = hidePos;
        }

        void ActionOnDestroy(GameObject obj)
        {
            if (_onDestroy == null)
                GameObject.Destroy(obj);
            else
                _onDestroy(obj);
        }

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

        public void Clear()
        {
            GameObject.DestroyImmediate(_poolContainer);
            _pool.Clear();
        }
    }
}