using UnityEngine;
using System.Collections.Generic;
using System;

namespace PiFramework
{
    public class PiProgress
    {
        List<PiProgress> _childList;
        List<object> _lockers;

        //public readonly float MinDuration;
        //public float TimeOut = 0f;
        object _owner;
        public TimeMode TimeMode = TimeMode.UnScale;

        /// <summary>
        /// current progress in range 0,1
        /// </summary>
        public float Progress;
        float _startTime;
        bool _started;
        bool _isDone;
        public event Action OnDone;
        protected PiProgress _parent;

        /// <summary>
        /// when owner become null, progress will be destroyed
        /// </summary>
        /// <param name="owner"></param>
        public PiProgress(object owner)
        {
            _owner = owner;
        }

        /*
        public PiProgress(float minDuration)
        {
            MinDuration = minDuration;
        }
        */

        public float Elapsed
        {
            get
            {
                if (TimeMode == TimeMode.UnScale)
                    return Time.unscaledTime - _startTime;
                else if (TimeMode == TimeMode.Scale)
                    return Time.time - _startTime;
                else
                    return Time.realtimeSinceStartup - _startTime;
            }
        }


        public void Start()
        {
            if (_started)
                return;
            if (TimeMode == TimeMode.UnScale)
                _startTime = Time.unscaledTime;
            else if (TimeMode == TimeMode.Scale)
                _startTime = Time.time;
            else
                _startTime = Time.realtimeSinceStartup;
            _started = true;
            ProgressManager.Add(this);
        }

        public void Lock(object locker)
        {
            if (_lockers == null)
                _lockers = new List<object>();
            _lockers.Add(locker);
        }

        public void Unlock(object locker)
        {
            if (_lockers != null)
            {
                _lockers.Remove(locker);
            }
        }

        public void CompleteSelf() {
            _isDone = true;
        }

        public void Stop()
        {
            if (_started)
            {
                _started = false;
                ProgressManager.Remove(this);
            }
        }

        public void AddChild(PiProgress childProgress)
        {
            if (_childList == null)
                _childList = new List<PiProgress>();
            _childList.Add(childProgress);
            childProgress._parent = this;
            
        }

        public void RemoveChild(PiProgress childProgress)
        {
            if (_childList != null)
                _childList.Remove(childProgress);
            childProgress._parent = null;
        }

        public void Destroy()
        {
            Stop();
            if (_parent != null)
            {
                _parent.RemoveChild(this);
            }

            if (_childList != null)
                _childList.Clear();
            if (_lockers != null)
                _lockers.Clear();
        }

        public bool IsDone
        {
            
            get {
                if (_lockers != null && _lockers.Count > 0)
                    return false;

                if (_childList != null)
                {
                    foreach (var p in _childList)
                    {
                        if (!p.IsDone)
                            return false;
                    }
                }
                return _isDone;
            }
        }

        
        protected class ProgressManager
        {
            static List<PiProgress> _progresses = new List<PiProgress>();
            static bool _initialized = false;
            public static void Add(PiProgress progress){
                Init();
                _progresses.Add(progress);
            }

            public static void Remove(PiProgress progress)
            {
                Init();
                _progresses.Remove(progress);
            }
            static void Init(){
                if (_initialized)
                    return;
                _initialized = true;
                PiCore.instance.systemEvents.BeginLateUpdate.AddListener(HandleStartLateUpdate);
                //Pi.AddEventHandler(BaseEvent.StartLateUpdate, HandleStartLateUpdate);
               
            }

            static void HandleStartLateUpdate()
            {        
                for (var i = 0; i < _progresses.Count; i++ )
                {
                    if (_progresses[i]._owner == null)
                    {
                        _progresses[i].Destroy();
                        i--;
                    }
                    else
                    {
                        if (_progresses[i].OnDone != null && _progresses[i].IsDone)
                        {
                            _progresses[i].OnDone.Invoke();
                            _progresses[i].Stop();
                            i--;
                        }
                    }
                }
            }
            /*
            static ProgressManager _instance;
            public static ProgressManager Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new ProgressManager();
                    return _instance;
                }
            }
            */

        }
    }
}