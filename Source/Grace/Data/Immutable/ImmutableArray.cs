using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grace.Data.Immutable
{
    /// <summary>
    /// Immutable array create methods
    /// </summary>
    public static class ImmutableArray
    {
        /// <summary>
        /// Create an array 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ImmutableArray<T> Create<T>(params T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return ImmutableArray<T>.Empty;
            }

            return new ImmutableArray<T>(CloneArray(array));
        }

        /// <summary>
        /// Creates a new immutable list from an IEnumerable(T)
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="list">list of T</param>
        /// <returns>immutable array</returns>
        public static ImmutableArray<T> From<T>(IEnumerable<T> list)
        {
            T[] array = list.ToArray();

            return  new ImmutableArray<T>(array);
        }

        private static T[] CloneArray<T>(T[] array)
        {
            T[] returnValue = new T[array.Length];

            Array.Copy(returnValue, 0, array, 0, array.Length);

            return returnValue;
        }
    }

    /// <summary>
    /// Immutable List that implements IReadOnlyList(T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ImmutableArray<T> : IReadOnlyList<T>
    {
        private readonly T[] _list;

        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly ImmutableArray<T> Empty;

        static ImmutableArray()
        {
            Empty = new ImmutableArray<T>(new T[0]);
        }

        internal ImmutableArray(T[] list)
        {
            _list = list;
        }

        /// <summary>
        /// Get an enumerator for this list
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new ImmutableArrayEnumerator<T>(_list);
        }

        /// <summary>
        /// Get an enumerator for this list
        /// </summary>
        /// <returns>enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Contains specified value
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < _list.Length; i++)
            {
                if (Equals(item, _list[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copy to array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_list, 0, array, arrayIndex, _list.Length);
        }

        /// <summary>
        /// Count of list
        /// </summary>
        public int Count
        {
            get { return _list.Length; }
        }

        /// <summary>
        /// Immutable list is always read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Index of item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _list.Length; i++)
            {
                if (Equals(item, _list[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// Index into list
        /// </summary>
        /// <param name="index">index for list</param>
        /// <returns>T at index</returns>
        public T this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Add T to ImmutableList creating new list
        /// </summary>
        /// <param name="value">new T</param>
        /// <returns>new list</returns>
        public ImmutableArray<T> Add(T value)
        {
            int listLength = _list.Length;
            T[] newArray = new T[listLength + 1];

            Array.Copy(_list, 0, newArray, 0, listLength);

            newArray[listLength] = value;

            return new ImmutableArray<T>(newArray);
        }

        /// <summary>
        /// Add and IEnumerable to this list
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public ImmutableArray<T> AddRange(IEnumerable<T> enumerable)
        {
            return AddRange(enumerable.ToArray());
        }

        /// <summary>
        /// Adds a range of T to immutable list creating a new one
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ImmutableArray<T> AddRange(params T[] value)
        {
            int listLength = _list.Length;
            int valueLength = value.Length;
            T[] newArray = new T[listLength + valueLength];

            Array.Copy(_list, 0, newArray, 0, listLength);
            Array.Copy(value, 0, newArray, listLength, valueLength);

            return new ImmutableArray<T>(newArray);
        }

        /// <summary>
        /// Implicit conversion from List(T)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static implicit operator ImmutableArray<T>(List<T> list)
        {
            return new ImmutableArray<T>(list.ToArray());
        }

        /// <summary>
        /// Implicit conversion to List(T)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static implicit operator List<T>(ImmutableArray<T> list)
        {
            return new List<T>(list._list);
        }

        /// <summary>
        /// Internal enumerator class used to enumerate lists, stacks and queues
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal class ImmutableArrayEnumerator<T> : IEnumerator<T>
        {
            private int _count = -1;
            private readonly T[] _list;

            public ImmutableArrayEnumerator(T[] list)
            {
                _list = list;
            }

            public bool MoveNext()
            {
                if (_count + 1 >= _list.Length)
                {
                    return false;
                }

                _count++;

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public T Current
            {
                get { return _list[_count]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {

            }
        }
    }
}
