using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PiFramework
{
    public sealed class PendingInvoker
    {
        // Start is called before the first frame update
        UnityAction _callbacks;
        float _timer;
        //bool _autoReset;
        //int _loop;
        float _interval;
        bool _enabled;
        List<object> _keys;

        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public PendingInvoker(float interval = 0)
        {
            PiBase.systemEvents.BeginUpdate.AddListener(Update);
            _interval = interval;
            _timer = interval;
            //_loop = 1;
            //_autoReset = false;
        }

        public float remainingTime
        {
            get
            {
                return _timer;
            }
        }
        public PendingInvoker SetTimer(float t)
        {
            _timer = t;
            return this;
        }

        public PendingInvoker AddCallback(UnityAction call)
        {
            _callbacks += call;
            return this;
        }

        public PendingInvoker RemoveCallback(UnityAction call)
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
                    _callbacks.Invoke();
                    PiBase.systemEvents.BeginUpdate.RemoveListener(Update);
                    Dispose();
                }
            }
        }

        void Dispose()
        {
            _callbacks = null;
            _keys = null;
        }

        public void Start()
        {
            enabled = true;
        }

        public void Stop()
        {
            enabled = false;
        }
    }
}