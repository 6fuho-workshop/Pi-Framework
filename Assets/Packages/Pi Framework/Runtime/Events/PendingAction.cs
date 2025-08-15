using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PF
{
    public sealed class PendingAction
    {
        Action callbacks;
        float timer;
        object host;
        List<object> keyList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">The owner of Action, if host became null Action will be canceled</param>
        /// <param name="call"></param>
        /// <param name="timer">If timer <= 0, Action will delay 1 frame</param>
        public PendingAction(object host, Action call, float timer = 0)
        {
            P.SystemEvents.OnFirstUpdate.Register(Update);
            callbacks += call;
            this.host = host;
            this.timer = timer;
        }

        public void AddAction(Action call)
        {
            callbacks += call;
        }

        public void RemoveAction(Action call)
        {
            callbacks -= call;
        }

        public PendingAction Lock(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            keyList ??= new List<object>();
            keyList.Add(key);
            return this;
        }

        public PendingAction Unlock(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            keyList ??= new List<object>();
            keyList.Remove(key);
            return this;
        }

        void Update()
        {
            if (host == null)
            {
                Destroy();
                return;
            }

            timer -= UnityEngine.Time.deltaTime;
            if (timer > 0f)
            {
                return;
            }
            else
            {
                //RemoveCallback null lockers
                bool locked = false;
                if (keyList != null)
                {
                    keyList.RemoveAll(item => item == null);
                    locked = keyList.Count > 0;
                }

                //Invoke
                if (!locked)
                {
                    callbacks.Invoke();
                    Destroy();
                }
            }
        }

        void Destroy()
        {
            P.SystemEvents.OnFirstUpdate.Unregister(Update);
            callbacks = null;
            keyList = null;
            host = null;
        }
    }

    public class HookableEvent : PiEvent<PendingAction> { }
}