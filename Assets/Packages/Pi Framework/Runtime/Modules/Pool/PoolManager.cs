using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PiFramework.Pool
{
    public class PoolManager : PiModule
    {
        readonly Vector3 hidePos = Vector3.left * 100000f;

        const string objectPoolsParentName = "[ObjectPool]";

        const string persistentObjectPoolsParentName = "[PersistentPool]";

        Dictionary<Poolable, ObjectPool<GameObject>> _pools = new();

        Transform _poolContainer;
        Transform poolContainer
        {
            get
            {
                if (_poolContainer == null)
                {
                    _poolContainer = new GameObject(objectPoolsParentName).transform;
                }
                return _poolContainer;
            }
        }

        Transform _persistentPoolContainer;
        Transform persistentPoolContainer
        {
            get
            {
                if (_persistentPoolContainer == null)
                {
                    var go = new GameObject(persistentObjectPoolsParentName);
                    DontDestroyOnLoad(go);
                    _persistentPoolContainer = go.transform;
                }
                return _persistentPoolContainer;
            }
        }

        protected override void Initialize()
        {
            //var p = GetPool(gameObject);

        }

        public ObjectPool<GameObject> GetPool(GameObject prefab)
        {
            var poolable = prefab.GetComponent<Poolable>();
            if (poolable == null)
            {
                Debug.LogError("prefab \"" + prefab.name + "\" is not poolable");
                return null;
            }
            return GetPool(poolable);
        }

        public void Preload(GameObject prefab)
        {
            var poolable = prefab.GetComponent<Poolable>();
            if (poolable == null)
            {
                Debug.LogError("prefab \"" + prefab.name + "\" is not poolable");
                return;
            }

            var count = poolable.preloadCount;
            var pool = GetPool(poolable);
            count -= pool.CountInactive;
            if (count <= 0)
                return;

            var tmpArray = new GameObject[count];

            for(int i = 0; i < count; i++)
                tmpArray[i] = pool.Get();

            for (int i = 0; i < count; i++)
                pool.Release(tmpArray[i]);
        }

        ObjectPool<GameObject> GetPool(Poolable Poolable)
        {
            ObjectPool<GameObject> pool;

            GameObject prefab = Poolable.gameObject;

            if (!_pools.TryGetValue(Poolable, out pool))
            {
                pool = CreatePool(prefab, Poolable);
                _pools.Add(Poolable, pool);
            }

            return pool;
        }

        ObjectPool<GameObject> CreatePool(GameObject prefab, Poolable poolable)
        {
            var builder = new PoolBuilder<GameObject>();
            return builder.DefaultCapacity(poolable.defaultCapacity)
                .MaxSize(poolable.maxSize)
                .CreateFunc(() =>
                {
                    var go = Instantiate(prefab);
                    if (poolable.setParentOnCreate)
                        go.transform.parent = poolable.dontDestroyOnLoad ? persistentPoolContainer : poolContainer;
                    return go;
                })
                .ActionOnRelease((o) =>
                {
                    o.transform.parent = poolable.dontDestroyOnLoad ? persistentPoolContainer : poolContainer;
                    if (poolable.deactiveOnRelease)
                        o.SetActive(false);
                    if (poolable.hideOnRelease)
                        o.transform.position = hidePos;
                })
                .ActionOnDestroy((o) => { Destroy(o); })
                .Build();
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }
            _pools.Clear();
        }
    }
}