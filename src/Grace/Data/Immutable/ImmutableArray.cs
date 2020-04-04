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

            return new ImmutableArray<T>(CloneArray(array, array.Length));
        }

        /// <summary>
        /// Creates a new immutable list from an IEnumerable(T)
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="list">list of T</param>
        /// <returns>immutable array</returns>
        public static ImmutableArray<T> From<T>(IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            var array = list.ToArray();

            return new ImmutableArray<T>(array);
        }

        /// <summary>
        /// Create a new immutable array from an T[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ImmutableArray<T> From<T>(T[] ts, int count = -1)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            return new ImmutableArray<T>(CloneArray(ts, count));
        }

        internal static T[] CloneArray<T>(T[] array, int length)
        {
            if (length < 0)
            {
                length = array.Length;
            }

            var returnValue = new T[length];

            Array.Copy(array, 0, returnValue, 0, length);

            return returnValue;
        }
    }

    /// <summary>
    /// Immutable List that implements IReadOnlyList(T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ImmutableArray<T> : IReadOnlyList<T>, IEqualityComparer<ImmutableArray<T>>, IStructuralComparable, IStructuralEquatable
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
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < _list.Length; i++)
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
        /// Length of the array
        /// </summary>
        public int Count => _list.Length;

        /// <summary>
        /// Length of the array
        /// </summary>
        public int Length => _list.Length;

        /// <summary>
        /// Immutable list is always read only
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Index of item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (var i = 0; i < _list.Length; i++)
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
        public T this[int index] => _list[index];

        /// <summary>
        /// Add T to ImmutableList creating new list
        /// </summary>
        /// <param name="value">new T</param>
        /// <returns>new list</returns>
        public ImmutableArray<T> Add(T value)
        {
            var listLength = _list.Length;
            var newArray = new T[listLength + 1];

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
            var listLength = _list.Length;
            var valueLength = value.Length;
            var newArray = new T[listLength + valueLength];

            Array.Copy(_list, 0, newArray, 0, listLength);
            Array.Copy(value, 0, newArray, listLength, valueLength);

            return new ImmutableArray<T>(newArray);
        }

        /// <summary>
        /// Insert value into array
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ImmutableArray<T> Insert(int index, T value)
        {
            var listLength = _list.Length;
            var newArray = new T[listLength + 1];

            if (index == _list.Length)
            {
                Array.Copy(_list, 0, newArray, 0, listLength);

                newArray[listLength] = value;
            }
            else
            {
                Array.Copy(_list, 0, newArray, 0, index);

                newArray[index] = value;

                Array.Copy(_list, index, newArray, index + 1, listLength - index);
            }

            return new ImmutableArray<T>(newArray);
        }

        /// <summary>
        /// Implicit conversion from List(T)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static explicit operator ImmutableArray<T>(T[] list)
        {
            if (list == null || list.Length == 0)
            {
                return Empty;
            }

            return new ImmutableArray<T>(ImmutableArray.CloneArray(list, list.Length));
        }

        /// <summary>
        /// Implicit conversion to List(T)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static explicit operator T[] (ImmutableArray<T> list)
        {
            if (list._list != null)
            {
                return ImmutableArray.CloneArray(list._list, list._list.Length);
            }

            return Empty._list;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ImmutableArray<T> immutableArray)
            {
                return Equals(immutableArray, this);
            }

            return false;
        }

        /// <summary>
        /// Get hashcode of array
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_list != null)
            {
                return _list.GetHashCode();
            }

            return 0;
        }

        /// <summary>
        /// Compare arrays
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            var compareArray = other as Array;

            if (compareArray == null && other is ImmutableArray<T> immutableArray)
            {
                compareArray = immutableArray._list;

                if (compareArray == null && _list == null)
                {
                    return 0;
                }

                if (compareArray == null ^ _list == null)
                {
                    throw new ArgumentNullException(nameof(other), "Arrays not initialized properly");
                }
            }

            if (compareArray != null)
            {
                return ((IStructuralComparable)compareArray).CompareTo(_list, comparer);
            }

            throw new ArgumentNullException(nameof(other), "Other is not an array of T");
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
        {
            if (x._list.Length != y._list.Length)
            {
                return false;
            }

            for (var i = 0; i < x._list.Length; i++)
            {
                if (x._list[i] == null)
                {
                    if (y._list[i] != null)
                    {
                        return false;
                    }
                }
                else if (!x._list[i].Equals(y._list[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public int GetHashCode(ImmutableArray<T> obj)
        {
            if (obj._list != null)
            {
                return obj._list.GetHashCode();
            }

            return 0;
        }

        bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var compareArray = other as Array;

            if (compareArray == null && other is ImmutableArray<T> immutableArray)
            {
                compareArray = immutableArray._list;

                if (compareArray == null && _list == null)
                {
                    return false;
                }

                if (compareArray == null ^ _list == null)
                {
                    throw new ArgumentNullException(nameof(other), "Arrays not initialized properly");
                }
            }

            if (compareArray != null)
            {
                return ((IStructuralEquatable)compareArray).Equals(_list, comparer);
            }

            throw new ArgumentNullException(nameof(other), "Other is not an array of T");
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            IStructuralEquatable structuralEquatable = _list;

            return structuralEquatable?.GetHashCode(comparer) ?? GetHashCode();
        }


        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="left">left side of statement</param>
        /// <param name="right">right side of statement</param>
        /// <returns>true if equal</returns>
        public static bool operator ==(ImmutableArray<T> left, ImmutableArray<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Not equal override
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ImmutableArray<T> left, ImmutableArray<T> right)
        {
            return !left.Equals(right);
        }


        /// <summary>
        /// Equal override for nullable
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ImmutableArray<T>? left, ImmutableArray<T>? right)
        {
            return left.GetValueOrDefault().Equals(right.GetValueOrDefault());
        }

        /// <summary>
        /// Not equal override for nullable
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ImmutableArray<T>? left, ImmutableArray<T>? right)
        {
            return !left.GetValueOrDefault().Equals(right.GetValueOrDefault());
        }

        #region

        /// <summary>
        /// Internal enumerator class used to enumerate lists, stacks and queues
        /// </summary>
        /// <typeparam name="TItter"></typeparam>
        internal class ImmutableArrayEnumerator<TItter> : IEnumerator<TItter>
        {
            private int _count = -1;
            private readonly TItter[] _list;

            public ImmutableArrayEnumerator(TItter[] list)
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

            public TItter Current => _list[_count];

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }
        }
        #endregion
    }
}
