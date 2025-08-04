using ClockStone;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Demo.PoolObjectDemo
{
    public class PoolObjectDemo : MonoBehaviour
    {
        public SpriteRenderer spritePoolable;
        GameObject[] pooled;
        public int count = 20;
        void Start()
        {
            pooled = new GameObject[count];
            var go = spritePoolable.gameObject;
            var pool = Pi.Pool.GetPool(go);

            Pi.Pool.Preload(go);

            /*
            for (var i = 0; i < count; i++)
            {
                pooled[i] = pool.Get();
                pooled[i].transform.position = Vector3.zero;
            }

            for (var i = 0; i < 10; i++)
            {
                pool.Release(pooled[i]);
            }*/
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}