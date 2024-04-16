using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PiFramework.Pool
{
    public class PoolManager : PiModule
    {
        Dictionary<int, GameObjectPool> _pools = new();

        public GameObjectPool GetPool(GameObject poolablePrefab)
        {
            var instanceID = poolablePrefab.GetInstanceID();
            var pool = GetPool(instanceID);
            if(pool == null)
            {
                var poolable = poolablePrefab.GetComponent<Poolable>();
                if (poolable == null)
                {
                    Debug.LogError("prefab \"" + poolablePrefab.name + "\" is not poolable");
                    return null;
                }
                pool = new GameObjectPool(poolable);
                pool.poolID = instanceID;
                _pools.Add(instanceID, pool);
            }

            return pool;
        }

        internal GameObjectPool GetPool(int poolID)
        {
            if (_pools.TryGetValue(poolID, out GameObjectPool pool))
            {
                return pool;
            }

            return null;
        }

        public void ReleasePoolable(GameObject pooledGameObject) {
            var poolable = pooledGameObject.GetComponent<Poolable>();
            if (poolable == null)
            {
                Debug.LogError("gameObject \"" + pooledGameObject.name + "\" is not poolable");
                return;
            }

            var pool = GetPool(poolable.poolID);

            if (pool == null)
                Debug.LogError("Poolable Object \"" + poolable.name + "\" is not instantiated from pool manager");
            else
                pool.Release(pooledGameObject);
        }

        public void Preload(GameObject poolablePrefab)
        {
            GetPool(poolablePrefab).Preload();
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }
            _pools.Clear();
        }
    }
}