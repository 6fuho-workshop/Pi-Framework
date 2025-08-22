using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace PF.Primitives
{
    public static class ThrowExtensions
    {
        #region Null Checks

        /// <summary>
        /// Throw ArgumentNullException nếu value là null (class hoặc nullable struct).
        /// Với struct non-nullable check này luôn false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>(this T obj, string? paramName = null, string? message = null)
        {
            // UnityEngine.Object special case
#if UNITY_5_3_OR_NEWER
            if (obj is UnityEngine.Object unityObj)
            {
                if (unityObj == null) // Unity operator check
                    throw new ArgumentNullException(paramName ?? nameof(obj), message ?? "Unity Object is destroyed or null");
                return obj;
            }
#endif

            // Normal .NET object
            if (obj is null)
                throw new ArgumentNullException(paramName ?? nameof(obj), message);
            return obj;
        }

        /// <summary>
        /// Throw ObjectDisposedException nếu đối tượng đã bị dispose.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfDisposed<T>(this T value, bool isDisposed, string? objectName = null) where T : class
        {
            if (isDisposed)
                throw new ObjectDisposedException(objectName ?? typeof(T).Name);
            return value;
        }

        #endregion

        #region String Checks

        /// <summary>
        /// Throw ArgumentException nếu string rỗng hoặc null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfNullOrEmpty(this string? value, string? paramName = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("String must not be null or empty", paramName ?? nameof(value));
            return value!;
        }

        /// <summary>
        /// Throw ArgumentException nếu string null, empty hoặc chỉ toàn whitespace.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ThrowIfNullOrWhiteSpace(this string? value, string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String must not be null or whitespace", paramName ?? nameof(value));
            return value!;
        }

        #endregion

        #region Collection Checks

        /// <summary>
        /// Throw ArgumentException nếu collection null hoặc không có phần tử.
        /// Phiên bản hiệu quả hơn cho ICollection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<T> ThrowIfNullOrEmpty<T>(this ICollection<T>? value, string? paramName = null)
        {
            if (value is null)
                throw new ArgumentNullException(paramName ?? nameof(value));
            
            if (value.Count == 0)
                throw new ArgumentException("Collection must not be empty", paramName ?? nameof(value));
                
            return value;
        }

        /// <summary>
        /// Throw ArgumentException nếu collection null hoặc không có phần tử.<br/>
        /// Lưu ý nhỏ: Kiểm tra empty sẽ khiến enumeration xảy ra một lần, có thể tốn hiệu năng với lazy sequences.
        /// Cái này chỉ tốn hiệu năng thôi còn logic vẫn đảm bảo
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T>? value, string? paramName = null)
        {
            if (value is null)
                throw new ArgumentNullException(paramName ?? nameof(value));

            using var enumerator = value.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException("Collection must not be empty", paramName ?? nameof(value));

            return value;
        }

        #endregion

        #region Numeric Range Checks

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu số không nằm trong range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ThrowIfOutOfRange(this int value, int min, int max, string? paramName = null)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value,
                    $"Value must be between {min} and {max}");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu số không nằm trong range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ThrowIfOutOfRange(this double value, double min, double max, string? paramName = null)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value,
                    $"Value must be between {min} and {max}");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu số không nằm trong range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ThrowIfOutOfRange(this float value, float min, float max, string? paramName = null)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value,
                    $"Value must be between {min} and {max}");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu giá trị âm.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ThrowIfNegative(this int value, string? paramName = null)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value, "Value cannot be negative");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu giá trị âm.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ThrowIfNegative(this float value, string? paramName = null)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value, "Value cannot be negative");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu giá trị âm hoặc bằng không.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ThrowIfNegativeOrZero(this int value, string? paramName = null)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value, "Value must be positive");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu giá trị âm hoặc bằng không.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ThrowIfNegativeOrZero(this float value, string? paramName = null)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(value), value, "Value must be positive");
            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException nếu index nằm ngoài phạm vi hợp lệ [0, count-1].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ThrowIfIndexOutOfRange(this int index, int count, string? paramName = null)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(index), index,
                    $"Index must be between 0 and {count - 1}");
            return index;
        }

        #endregion

        #region Validation Checks

        /// <summary>
        /// Throw InvalidOperationException nếu điều kiện không hợp lệ.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfInvalid<T>(this T value, Func<T, bool> predicate, string? message = null)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (!predicate(value))
                throw new InvalidOperationException(message ?? $"Invalid value: {value}");
            return value;
        }

        /// <summary>
        /// Throw ArgumentException nếu enum value không được định nghĩa.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ThrowIfNotDefined<TEnum>(this TEnum value, string? paramName = null) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentException($"Value {value} is not defined in enum {typeof(TEnum).Name}",
                    paramName ?? nameof(value));
            return value;
        }

        #endregion
    }
}
