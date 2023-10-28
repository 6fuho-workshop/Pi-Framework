
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Progress;

namespace PiFramework
{
    public static class Dev
    {
        [Conditional("DEBUG")]
        public static void LogIf(bool condition, object message, Object context = null)
        {
            if (condition)
                UnityEngine.Debug.Log(message, context);
        }

        [Conditional("DEBUG")]
        public static void Ensure(bool condition, Object context = null)
        {
            Ensure(condition, "Ensure failed.", context);
        }

        [Conditional("DEBUG")]
        public static void Ensure(bool condition, string message, Object context = null)
        {
            if (condition)
                return;

            string text = StackTraceUtility.ExtractStackTrace();
            UnityEngine.Debug.Assert(false, message + "\n" + text, context);
        }
    }
}