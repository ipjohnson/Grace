using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Grace.Diagnostics;

namespace Grace.Data.Immutable
{
    /// <summary>
    /// Static helper class for linked list
    /// </summary>
    public static class ImmutableLinkedList
    {

        /// <summary>
        /// Empty an immutable linked list in a thread and return the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list to empty</param>
        /// <returns></returns>
        public static ImmutableLinkedList<T> ThreadSafeEmpty<T>(ref ImmutableLinkedList<T> list)
        {
            return Interlocked.Exchange(ref list, ImmutableLinkedList<T>.Empty);
        }

        /// <summary>
        /// Add to a list in a thread safe manner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void ThreadSafeAdd<T>(ref ImmutableLinkedList<T> list, T value)
        {
            var listValue = list;
            var newList = listValue.Add(value);

            if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
            {
                return;
            }

            AddWithWait(ref list, value);
        }

        private static void AddWithWait<T>(ref ImmutableLinkedList<T> list, T value)
        {
            ImmutableLinkedList<T> listValue;
            ImmutableLinkedList<T> newList;
            var wait = new SpinWait();

            while (true)
            {
                wait.SpinOnce();

                listValue = list;
                newList = listValue.Add(value);

                if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Add a range to a list in a thread safe manner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="values"></param>
        public static void ThreadSafeAddRange<T>(ref ImmutableLinkedList<T> list, IEnumerable<T> values)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (values == null) throw new ArgumentNullException(nameof(values));

            foreach (var value in values)
            {
                ThreadSafeAdd(ref list, value);
            }
        }

        /// <summary>
        /// creates a new immutable linked list from an enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">enumerable of T</param>
        /// <returns>new list</returns>
        public static ImmutableLinkedList<T> From<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            return ImmutableLinkedList<T>.Empty.AddRange(enumerable);
        }

        /// <summary>
        /// Create a new immutable linked list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">values to create list from</param>
        /// <returns></returns>
        public static ImmutableLinkedList<T> Create<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                return ImmutableLinkedList<T>.Empty;
            }

            return ImmutableLinkedList<T>.Empty.AddRange(values);
        }
    }

    /// <summary>
    /// Immutable linked list class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayString) + ",nq}")]
    [DebuggerTypeProxy(typeof(ImmutableLinkedListDebugView<>))]
    public class ImmutableLinkedList<T> : IEnumerable<T>, IReadOnlyList<T>
    {
        /// <summary>
        /// Empty instance of list
        /// </summary>
        public static readonly ImmutableLinkedList<T> Empty = new ImmutableLinkedList<T>(default(T), null, 0);

        /// <summary>
        /// Empty enumerator
        /// </summary>
        private static readonly EmptyLinkedListEnumerator EmptyEnumerator = new EmptyLinkedListEnumerator();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="next"></param>
        /// <param name="count"></param>
        private ImmutableLinkedList(T value, ImmutableLinkedList<T> next, int count)
        {
            Value = value;
            Next = next;
            Count = count;
        }

        /// <summary>
        /// For for link
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Next link in the list
        /// </summary>
        public ImmutableLinkedList<T> Next { get; }

        /// <summary>
        /// Count for list
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Add value to list
        /// </summary>
        /// <param name="value">value to add to list</param>
        /// <returns>new linked list</returns>
        public ImmutableLinkedList<T> Add(T value)
        {
            return new ImmutableLinkedList<T>(value, this, Count + 1);
        }

        /// <summary>
        /// Add range to linked list
        /// </summary>
        /// <param name="range">range to add</param>
        /// <returns>new linked list</returns>
        public ImmutableLinkedList<T> AddRange(IEnumerable<T> range)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));

            var current = this;

            foreach (var value in range)
            {
                current = current.Add(value);
            }

            return current;
        }

        /// <summary>
        /// Get an enumerator for list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Count == 0 ? (IEnumerator<T>)EmptyEnumerator : new LinkedListEnumerator(this);
        }

        /// <summary>
        /// Get enumerator for list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Reverse linked list
        /// </summary>
        /// <returns></returns>
        public ImmutableLinkedList<T> Reverse()
        {
            return this.Aggregate(Empty, (current, t) => current.Add(t));
        }

        /// <summary>
        /// Visit all values in list
        /// </summary>
        /// <param name="action">action to call</param>
        /// <param name="startAtEnd">start at the end of the linked list</param>
        public void Visit(Action<T> action, bool startAtEnd = false)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (this == Empty)
            {
                return;
            }

            if (!startAtEnd)
            {
                action(Value);
            }

            if (Next != Empty)
            {
                Next.Visit(action, startAtEnd);
            }

            if (startAtEnd)
            {
                action(Value);
            }
        }

        /// <summary>
        /// Check if list contains value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            if (this == Empty)
            {
                return false;
            }

            var current = this;

            while (current != null && current != Empty)
            {
                if (value.Equals(current.Value))
                {
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        private string DebuggerDisplayString => $"Count: {Count}";

        /// <summary>
        /// Empty enumerator
        /// </summary>
        private class EmptyLinkedListEnumerator : IEnumerator<T>
        {
            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                return false;
            }

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {

            }

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public T Current { get; }

            /// <summary>Gets the current element in the collection.</summary>
            /// <returns>The current element in the collection.</returns>
            object IEnumerator.Current => Current;

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {

            }
        }

        private class LinkedListEnumerator : IEnumerator<T>
        {
            private readonly ImmutableLinkedList<T> _start;
            private ImmutableLinkedList<T> _current;

            public LinkedListEnumerator(ImmutableLinkedList<T> current)
            {
                _start = current;
            }

            public bool MoveNext()
            {
                if (_current == null)
                {
                    if (_start.Next == null)
                    {
                        return false;
                    }

                    _current = _start;
                }
                else if (_current.Next?.Next == null)
                {
                    return false;
                }
                else
                {
                    _current = _current.Next;
                }

                return true;
            }

            public void Reset()
            {
                _current = _start;
            }

            public T Current => _current.Value;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // leaving empty as there is nothing to dispose
            }
        }

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <returns>The element at the specified index in the read-only list.</returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                var current = this;

                for (var i = 0; i < index; i++)
                {
                    current = current.Next;
                }

                return current.Value;
            }
        }
    }
}
