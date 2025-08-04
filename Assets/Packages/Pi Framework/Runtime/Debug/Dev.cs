using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PiFramework
{
    /// <summary>
    /// Provides utility methods for development-time debugging and validation.
    /// These methods only execute in DEBUG builds and are automatically stripped from release builds.
    /// </summary>
    /// <remarks>
    /// Use these methods to add debug-only checks and logs that won't impact release performance.
    /// The Conditional attribute ensures these calls are removed during compilation for non-DEBUG builds.
    /// </remarks>
    public static class Dev
    {
        /// <summary>
        /// Logs a message to the Unity console if the specified condition is true.
        /// Only active in DEBUG builds.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <param name="message">The message to log if the condition is true</param>
        /// <param name="context">Optional Unity object to link to this log entry</param>
        /// <example>
        /// <code>
        /// Dev.LogIf(playerHealth &lt; 20, "Player health is critically low!", player);
        /// </code>
        /// </example>
        [Conditional("DEBUG")]
        public static void LogIf(bool condition, object message, Object context = null)
        {
            if (condition)
                UnityEngine.Debug.Log(message, context);
        }

        /// <summary>
        /// Ensures that a condition is true, showing a default assertion message if it's not.
        /// Only active in DEBUG builds.
        /// </summary>
        /// <param name="condition">The condition that should be true</param>
        /// <param name="context">Optional Unity object to link to this assertion</param>
        /// <example>
        /// <code>
        /// Dev.Ensure(playerHealth > 0);
        /// </code>
        /// </example>
        [Conditional("DEBUG")]
        public static void Ensure(bool condition, Object context = null)
        {
            Ensure(condition, "Ensure failed.", context);
        }

        /// <summary>
        /// Ensures that a condition is true, showing a custom assertion message if it's not.
        /// Only active in DEBUG builds.
        /// </summary>
        /// <param name="condition">The condition that should be true</param>
        /// <param name="message">Custom message to display if the condition is false</param>
        /// <param name="context">Optional Unity object to link to this assertion</param>
        /// <example>
        /// <code>
        /// Dev.Ensure(inventory.Contains(requiredItem), "Player is missing required item!", player);
        /// </code>
        /// </example>
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