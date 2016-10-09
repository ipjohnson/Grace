using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Grace.Data.Immutable
{
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
            var listValue = list;

            if (ReferenceEquals(Interlocked.CompareExchange(ref list, ImmutableLinkedList<T>.Empty, listValue), listValue))
            {
                return listValue;
            }

            var wait = new SpinWait();

            while (true)
            {
                wait.SpinOnce();

                listValue = list;

                if (ReferenceEquals(Interlocked.CompareExchange(ref list, ImmutableLinkedList<T>.Empty, listValue), listValue))
                {
                    return listValue;
                }
            }
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
            var newList = list.Add(value);

            if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
            {
                return;
            }

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
            var listValue = list;
            var newValues = values.ToArray();
            var newList = listValue.AddRange(newValues);

            if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
            {
                return;
            }

            var wait = new SpinWait();

            while (true)
            {
                wait.SpinOnce();

                listValue = list;
                newList = listValue.AddRange(newValues);

                if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
                {
                    return;
                }
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
            return ImmutableLinkedList<T>.Empty.AddRange(values);
        }

        /// <summary>
        /// Remove from list in a thread safe anner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list to remove from</param>
        /// <param name="value">value to remove from</param>
        public static void ThreadSafeRemove<T>(ref ImmutableLinkedList<T> list, T value)
        {
            var listValue = list;
            var newList = ImmutableLinkedList<T>.Empty.AddRange(listValue.Where(x => !value.Equals(x)).Reverse());

            if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
            {
                return;
            }

            var wait = new SpinWait();

            while (true)
            {
                wait.SpinOnce();

                listValue = list;
                newList = ImmutableLinkedList<T>.Empty.AddRange(listValue.Where(x => !value.Equals(x)).Reverse());

                if (ReferenceEquals(Interlocked.CompareExchange(ref list, newList, listValue), listValue))
                {
                    return;
                }
            }
        }
    }

    public class ImmutableLinkedList<T> : IEnumerable<T>
    {
        public static readonly ImmutableLinkedList<T> Empty = new ImmutableLinkedList<T>(default(T), null, 0);

        private ImmutableLinkedList(T value, ImmutableLinkedList<T> next, int count)
        {
            Value = value;
            Next = next;
            Count = count;
        }

        public T Value { get; }

        public ImmutableLinkedList<T> Next { get; }

        public int Count { get; }

        public ImmutableLinkedList<T> Add(T value)
        {
            return new ImmutableLinkedList<T>(value, this, Count + 1);
        }

        public ImmutableLinkedList<T> AddRange(IEnumerable<T> range)
        {
            var current = this;

            foreach (var value in range)
            {
                current = current.Add(value);
            }

            return current;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LinkedListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ImmutableLinkedList<T> Reverse()
        {
            return this.Aggregate(Empty, (current, t) => current.Add(t));
        }

        public void Visit(Action<T> action, bool startAtEnd = false)
        {
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

            }
        }
    }
}
