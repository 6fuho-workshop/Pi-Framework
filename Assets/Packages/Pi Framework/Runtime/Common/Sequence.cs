using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Common
{
    /// <summary>
    /// Represents a read-only sequence of reference type elements, supporting indexed access and traversal in both directions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence. Must be a reference type.</typeparam>
    public interface IReadOnlySequence<T> : IEnumerable<T> where T : class
    {
        /// <summary>
        /// Gets the number of elements contained in the sequence.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the first element in the sequence, or <c>null</c> if the sequence is empty.
        /// </summary>
        T First { get; }

        /// <summary>
        /// Gets the last element in the sequence, or <c>null</c> if the sequence is empty.
        /// </summary>
        T Last { get; }

        /// <summary>
        /// Determines whether the sequence contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns>True if the element is found; otherwise, false.</returns>
        bool Contains(T item);

        /// <summary>
        /// Returns the zero-based index of the specified element in the sequence.
        /// </summary>
        /// <param name="item">The element to locate.</param>
        /// <returns>The zero-based index if found; otherwise, -1.</returns>
        int IndexOf(T item);

        /// <summary>
        /// Gets the element at the specified zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        T GetAt(int index);

        /// <summary>
        /// Returns an enumerable for traversing the sequence from first to last.
        /// </summary>
        /// <returns>An enumerable of elements in forward order.</returns>
        IEnumerable<T> Traverse();

        /// <summary>
        /// Returns an enumerable for traversing the sequence from last to first.
        /// </summary>
        /// <returns>An enumerable of elements in reverse order.</returns>
        IEnumerable<T> TraverseBackward();
    }

    /// <summary>
    /// Represents a mutable sequence of reference type elements, supporting indexed access, insertion, removal, and traversal in both directions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence. Must be a reference type.</typeparam>
    public interface ISequence<T> : IReadOnlySequence<T> where T : class
    {
        /// <summary>
        /// Adds an element to the beginning of the sequence.
        /// </summary>
        /// <param name="item">The element to add.</param>
        /// <returns>The sequence itself for chaining.</returns>
        ISequence<T> Prepend(T item);

        /// <summary>
        /// Adds an element to the end of the sequence.
        /// </summary>
        /// <param name="item">The element to add.</param>
        /// <returns>The sequence itself for chaining.</returns>
        ISequence<T> Append(T item);

        /// <summary>
        /// Inserts an element before the specified target element.
        /// </summary>
        /// <param name="target">The element before which to insert.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>The sequence itself for chaining.</returns>
        ISequence<T> InsertBefore(T target, T item);

        /// <summary>
        /// Inserts an element after the specified target element.
        /// </summary>
        /// <param name="target">The element after which to insert.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>The sequence itself for chaining.</returns>
        ISequence<T> InsertAfter(T target, T item);

        /// <summary>
        /// Inserts an element at the specified zero-based index in the sequence.
        /// If the index is equal to the number of elements, the item is appended at the end.
        /// If the index is 0, the item is inserted at the beginning.
        /// Throws an exception if the index is out of range (less than 0 or greater than Count).
        /// Note: The index refers to the position of the item in the sequence after insertion.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the element.</param>
        /// <param name="item">The element to insert.</param>
        /// <returns>The element that was inserted.</returns>
        T InsertAt(int index, T item);

        /// <summary>
        /// Removes the specified element from the sequence.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>The sequence itself for chaining.</returns>
        ISequence<T> Remove(T item);

        /// <summary>
        /// Removes all elements from the sequence.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value indicating whether the sequence is read-only.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Returns a new read-only sequence that is a clone (shallow copy) of the current sequence.
        /// The returned sequence contains the same elements as the original, but modifications to one will not affect the other.
        /// This is useful for exposing a safe, immutable view of the sequence to consumers.
        /// </summary>
        /// <returns>A new <see cref="ISequence{T}"/> that is read-only and contains the same elements as this sequence.</returns>
        ISequence<T> AsReadOnly();
    }

    /// <summary>
    /// A simple, mutable sequence implementation backed by a <see cref="List{T}"/>.
    /// Supports indexed access, insertion, removal, and traversal in both directions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence. Must be a reference type.</typeparam>
    public class SimpleSequence<T> : ISequence<T> where T : class
    {
        /// <summary>
        /// The underlying list storing the sequence items.
        /// </summary>
        protected readonly List<T> _items = new();

        /// <summary>
        /// Indicates whether the sequence is read-only.
        /// </summary>
        protected bool _isReadOnly = false;

        /// <inheritdoc/>
        public virtual bool IsReadOnly => _isReadOnly;

        /// <inheritdoc/>
        public virtual int Count => _items.Count;

        /// <inheritdoc/>
        public virtual T First => _items.Count == 0 ? null : _items[0];

        /// <inheritdoc/>
        public virtual T Last => _items.Count == 0 ? null : _items[^1];

        /// <inheritdoc/>
        public virtual bool Contains(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _items.Contains(item);
        }

        /// <inheritdoc/>
        public virtual ISequence<T> AsReadOnly()
        {
            var copy = new SimpleSequence<T>();
            copy._items.AddRange(_items);
            copy.Lock();
            return copy;
        }

        /// <summary>
        /// Locks the sequence, making it read-only.
        /// </summary>
        protected virtual void Lock()
        {
            _isReadOnly = true;
        }

        /// <summary>
        /// Throws an exception if the sequence is read-only.
        /// </summary>
        protected void CheckReadOnly()
        {
            if (_isReadOnly)
                throw new InvalidOperationException("Sequence is read-only.");
        }

        /// <inheritdoc/>
        public virtual ISequence<T> Prepend(T item)
        {
            CheckReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Insert(0, item);
            return this;
        }

        /// <inheritdoc/>
        public virtual ISequence<T> Append(T item)
        {
            CheckReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
            return this;
        }

        /// <inheritdoc/>
        public virtual ISequence<T> InsertBefore(T existing, T newItem)
        {
            CheckReadOnly();
            if (existing == null) throw new ArgumentNullException(nameof(existing));
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));
            int index = _items.IndexOf(existing);
            if (index < 0)
                throw new ArgumentException("Existing item not found.", nameof(existing));
            _items.Insert(index, newItem);
            return this;
        }

        /// <inheritdoc/>
        public virtual ISequence<T> InsertAfter(T existing, T newItem)
        {
            CheckReadOnly();
            if (existing == null) throw new ArgumentNullException(nameof(existing));
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));
            int index = _items.IndexOf(existing);
            if (index < 0)
                throw new ArgumentException("Existing item not found.", nameof(existing));
            _items.Insert(index + 1, newItem);
            return this;
        }

        /// <inheritdoc/>
        public virtual T InsertAt(int index, T item)
        {
            CheckReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (index < 0 || index > _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and Count inclusive.");
            _items.Insert(index, item);
            return item;
        }

        /// <inheritdoc/>
        public virtual ISequence<T> Remove(T item)
        {
            CheckReadOnly();
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Remove(item);
            return this;
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            CheckReadOnly();
            _items.Clear();
        }

        /// <inheritdoc/>
        public virtual IEnumerable<T> Traverse()
        {
            return _items;
        }

        /// <inheritdoc/>
        public virtual IEnumerable<T> TraverseBackward()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
                yield return _items[i];
        }

        /// <inheritdoc/>
        public virtual int IndexOf(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _items.IndexOf(item);
        }

        /// <inheritdoc/>
        public virtual T GetAt(int index)
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException();
            return _items[index];
        }

        /// <inheritdoc/>
        public virtual IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Provides extension methods for <see cref="IReadOnlySequence{T}"/> and <see cref="ISequence{T}"/> to enhance sequence manipulation and querying.
    /// </summary>
    public static class SequenceExtensions
    {
        /// <summary>
        /// Gets the next element in the sequence after the specified current element, or <c>null</c> if there is none.
        /// </summary>
        public static T GetNextOf<T>(this IReadOnlySequence<T> sequence, T current) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (current == null) throw new ArgumentNullException(nameof(current));
            int idx = sequence.IndexOf(current);
            if (idx < 0 || idx + 1 >= sequence.Count)
                return null;
            return sequence.GetAt(idx + 1);
        }

        /// <summary>
        /// Gets the previous element in the sequence before the specified current element, or <c>null</c> if there is none.
        /// </summary>
        public static T GetPreviousOf<T>(this IReadOnlySequence<T> sequence, T current) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (current == null) throw new ArgumentNullException(nameof(current));
            int idx = sequence.IndexOf(current);
            if (idx <= 0)
                return null;
            return sequence.GetAt(idx - 1);
        }

        /// <summary>
        /// Removes all elements from the sequence that match the specified predicate.
        /// </summary>
        public static ISequence<T> RemoveWhere<T>(this ISequence<T> sequence, Func<T, bool> predicate) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var toRemove = new List<T>();
            foreach (var item in sequence)
                if (predicate(item)) toRemove.Add(item);

            foreach (var item in toRemove)
                sequence.Remove(item);

            return sequence;
        }

        /// <summary>
        /// Creates a shallow clone of the sequence.
        /// </summary>
        public static ISequence<T> Clone<T>(this ISequence<T> sequence) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            var copy = new SimpleSequence<T>();
            foreach (var item in sequence)
                copy.Append(item);
            return copy;
        }

        /// <summary>
        /// Appends a range of items to the end of the sequence.
        /// </summary>
        public static ISequence<T> AddRange<T>(this ISequence<T> sequence, IEnumerable<T> items) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (items == null) throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
                sequence.Append(item);
            return sequence;
        }

        /// <summary>
        /// Determines whether the specified item is the first element in the sequence.
        /// </summary>
        public static bool IsFirst<T>(this IReadOnlySequence<T> sequence, T item) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (item == null) throw new ArgumentNullException(nameof(item));
            return sequence.First == item;
        }

        /// <summary>
        /// Determines whether the specified item is the last element in the sequence.
        /// </summary>
        public static bool IsLast<T>(this IReadOnlySequence<T> sequence, T item) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (item == null) throw new ArgumentNullException(nameof(item));
            return sequence.Last == item;
        }

        /// <summary>
        /// Finds the closest element forward from the specified starting element that matches the given predicate.
        /// </summary>
        public static T FindClosestForward<T>(this IReadOnlySequence<T> sequence, T from, Func<T, bool> match) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (match == null) throw new ArgumentNullException(nameof(match));
            int idx = sequence.IndexOf(from);
            if (idx < 0) return null;
            for (int i = idx + 1; i < sequence.Count; i++)
            {
                var item = sequence.GetAt(i);
                if (match(item)) return item;
            }
            return null;
        }

        /// <summary>
        /// Copies the elements of the sequence to a new array.
        /// </summary>
        public static T[] ToArray<T>(this IReadOnlySequence<T> sequence) where T : class
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            var arr = new T[sequence.Count];
            for (int i = 0; i < sequence.Count; i++)
                arr[i] = sequence.GetAt(i);
            return arr;
        }
    }
}