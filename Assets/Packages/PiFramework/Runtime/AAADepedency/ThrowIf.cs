using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PF.Utils
{
    #nullable enable
    public static class ThrowIf
    {
        // ========= State validation =========

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if condition is false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void False(bool condition, string? message = null)
        {
            if (!condition)
                throw new InvalidOperationException(message ?? "Operation cannot be performed in the current state");
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if condition is true.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void True(bool condition, string? message = null)
        {
            if (condition)
                throw new InvalidOperationException(message ?? "Operation cannot be performed in the current state");
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if condition is false (argument check failed).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentFailed(bool condition, string? message = null)
        { 
            if (!condition)
                throw new ArgumentException(message ?? "Invalid argument: argument check failed");
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if condition is true (argument matches the check).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentMatch(bool condition, string? message = null)
        {
            if (condition)
                throw new ArgumentException(message ?? "Invalid argument: argument matches the check");
        }
    }
}