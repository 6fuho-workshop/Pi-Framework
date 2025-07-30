using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Timing
{
    public enum TimeType     {
        Scaled,
        Unscaled,
        Realtime
    }

    public class StopWatch
    {
        float startTime = -1;
        float _elapsed;
        //float _timestamp;

        protected virtual float CurrentTime() => UnityEngine.Time.time;
        public float elapsed
        {
            get
            {
                if (startTime > 0)
                {
                    return CurrentTime() - startTime;
                }
                else
                    return _elapsed;
            }
        }


        public void Start()
        {
            if (startTime < 0)
                startTime = CurrentTime() - elapsed;
        }


        public void Stop()
        {
            if (startTime > 0)
                _elapsed = CurrentTime() - startTime;
            startTime = -1;
        }


        public void Reset()
        {
            _elapsed = 0;
            startTime = -1;
        }
    }


    public class StopWatchUnscaled : StopWatch
    {
        protected override float CurrentTime() => UnityEngine.Time.unscaledTime;
    }


    public class StopWatchRealtime : StopWatch
    {
        protected override float CurrentTime() => UnityEngine.Time.realtimeSinceStartup;
    }
}