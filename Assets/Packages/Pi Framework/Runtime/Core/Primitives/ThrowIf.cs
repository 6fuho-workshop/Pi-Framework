using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PF.Core.Primitives
{
    // ThrowHelper.cs
    #nullable enable
    public static class ThrowIf
    {
        // ========= Null checks =========

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if argument is null (reference type).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Null<T>(T? argument, string? paramName = null) where T : class
        {
            if (argument is null)
                throw new ArgumentNullException(paramName ?? nameof(argument));
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if argument is null (nullable value type).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Null<T>(T? argument, string? paramName = null) where T : struct
        {
            if (!argument.HasValue)
                throw new ArgumentNullException(paramName ?? nameof(argument));
        }

        // ========= String checks =========

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if string is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullOrEmpty(string? argument, string? paramName = null)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentException("String cannot be null or empty", paramName ?? nameof(argument));
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if string is null, empty, or whitespace.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullOrWhiteSpace(string? argument, string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("String cannot be null, empty, or whitespace", paramName ?? nameof(argument));
        }

        // ========= Range checks =========

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is outside [min, max].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutOfRange(int argument, int min, int max, string? paramName = null)
        {
            if (argument < min || argument > max)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, $"Value must be between {min} and {max}");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is outside [min, max].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OutOfRange(float argument, float min, float max, string? paramName = null)
        {
            if (argument < min || argument > max)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, $"Value must be between {min} and {max}");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negative(int argument, string? paramName = null)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, "Value cannot be negative");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negative(float argument, string? paramName = null)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, "Value cannot be negative");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is zero or negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NegativeOrZero(int argument, string? paramName = null)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, "Value must be positive");
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if argument is zero or negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NegativeOrZero(float argument, string? paramName = null)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(argument), argument, "Value must be positive");
        }

        // ========= Collection checks =========

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if collection is null or empty.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullOrEmpty<T>(ICollection<T>? collection, string? paramName = null)
        {
            if (collection == null || collection.Count == 0)
                throw new ArgumentException("Collection cannot be null or empty", paramName ?? nameof(collection));
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if index is out of [0, count-1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IndexOutOfRange(int index, int count, string? paramName = null)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(index), index, $"Index must be between 0 and {count - 1}");
        }

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
                throw new ArgumentException(message ?? "Invalid argument");
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if condition is true (argument matches the check).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentMatch(bool condition, string? message = null)
        {
            if (condition)
                throw new ArgumentException(message ?? "Invalid argument");
        }
        // ========= Enum checks =========

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if value is not defined in enum.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotDefined<TEnum>(TEnum value, string? paramName = null) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentException($"Value {value} is not defined in enum {typeof(TEnum).Name}", paramName ?? nameof(value));
        }
    }
}