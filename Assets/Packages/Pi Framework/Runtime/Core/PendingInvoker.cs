using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PiFramework
{
    public sealed class PendingInvoker
    {
        Action _callbacks;
        float _timer;
        object _host;
        List<object> _keys;

        public PendingInvoker(object host, float timer = 0)
        {
            PiBase.systemEvents.beginUpdate.AddListener(Update);
            _timer = timer;
            _host = host;
        }

        public PendingInvoker AddCallback(Action call)
        {
            _callbacks += call;
            return this;
        }

        public PendingInvoker RemoveCallback(Action call)
        {
            _callbacks -= call;
            return this;
        }

        public PendingInvoker Lock(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_keys == null)
            {
                _keys = new List<object>();
            }
            _keys.Add(key);
            return this;
        }

        public PendingInvoker Unlock(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (_keys == null)
            {
                _keys = new List<object>();
            }
            _keys.Remove(key);
            return this;
        }

        void Update()
        {
            if (_host == null)
            {
                Destroy();
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0f)
            {
                return;
            }
            else
            {
                //RemoveCallback null lockers
                bool locked = false;
                if (_keys != null)
                {
                    _keys.RemoveAll(item => item == null);
                    locked = _keys.Count > 0;
                }

                //Invoke
                if (!locked)
                {
                    PiBase.systemEvents.beginUpdate.RemoveListener(Update);
                    _callbacks.Invoke();
                    Destroy();
                }
            }
        }

        void Destroy()
        {
            _callbacks = null;
            _keys = null;
            _host = null;
        }
    }
}