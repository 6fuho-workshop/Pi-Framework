using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public class StopWatch
    {
        float startTime = -1;
        float _elapsed;

        public float elapsed
        {
            get
            {
                if (startTime > 0)
                {
                    return _elapsed + Time.realtimeSinceStartup - startTime;
                }
                else
                    return _elapsed;
            }
        }


        public void Start()
        {
            if (startTime < 0)
                startTime = Time.realtimeSinceStartup;
        }


        public void stop()
        {
            if (startTime > 0)
                _elapsed += Time.realtimeSinceStartup - startTime;
            startTime = -1;
        }


        public void Reset()
        {
            _elapsed = 0;
            startTime = -1;
        }
    }
}