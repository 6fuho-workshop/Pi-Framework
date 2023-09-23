using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

namespace PiFramework
{
 
    /// <summary>
    /// Các method chỉ được build và hoạt động khi có define ENABLE_PROFILER (editor và development build)
    /// zero overhead when it is deployed in non-Development Build.
    /// </summary>
    public class PiProfiler
    {
        Stack<string> _sampleStack;
        Dictionary<string, Vector2> _result;
        bool _enable;

        public PiProfiler(){

        }

        public bool Enabled
        {
            get { return _enable; }
            set
            {
                //if (_enable == value)
                  //  return;
                _enable = value;
                if (_enable)
                    EnableProfiler();
                else
                    DisableProfiler();
            }
        }


        [Conditional("ENABLE_PROFILER")]
        public void BeginSample(string name)
        {
            if (!_enable) return;

            _result[name] = new Vector2(Time.realtimeSinceStartup, -1f);
            _sampleStack.Push(name);
            
        }

        [Conditional("ENABLE_PROFILER")]
        public void EndSample()
        {
            if (!_enable) return;
            if (_sampleStack.Count > 0)
            {
                var name = _sampleStack.Pop();
                var res = _result[name];
                res.y = Time.realtimeSinceStartup - res.x;
                _result[name] = res;
            }

        }

        [Conditional("ENABLE_PROFILER")]
        void EnableProfiler()
        {
            PiCore.instance.systemEvents.BeginOnGUI.AddListener(HandleOnGUI);
            _sampleStack = new Stack<string>();
            _result = new Dictionary<string, Vector2>();
        }

        [Conditional("ENABLE_PROFILER")]
        void DisableProfiler()
        {
            PiCore.instance.systemEvents.BeginOnGUI.RemoveListener(HandleOnGUI);
            _sampleStack.Clear();
            _result.Clear();
        }

        void HandleOnGUI()
        {

            /* dùng IMGUI build tăng thêm 1Mb
            GUIStyle headStyle = new GUIStyle();
            headStyle.fontSize = 30;
            GUILayout.Label("PROFILER v1.0", headStyle);
            foreach (KeyValuePair<string, Vector2> pair in _result)
            {
                if (pair.Value.y >= 0f)
                {
                    GUILayout.Label(pair.Key + ": " + pair.Value.y);
                }
            }
            */
            
        }


    }
}