using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grace.Data.Immutable
{
    /// <summary>
    /// Immutable List that implements IReadOnlyList(T)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ImmutableList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly T[] _list;

        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="list">list of T</param>
        public ImmutableList(params T[] list)
        {
            T[] newArray = new T[list.Length];

            Array.Copy(list, 0, newArray, 0, list.Length);

            _list = newArray;
        }

        /// <summary>
        /// Constructor takes an IEnumerable of T
        /// </summary>
        /// <param name="list"></param>
        public ImmutableList(IEnumerable<T> list)
        {
            _list = list.ToArray();
        }

        /// <summary>
        /// THis constructor is here as an internal cosntructor that doesn't copy the list passed in
        /// </summary>
        /// <param name="extraParam"></param>
        /// <param name="list"></param>
        private ImmutableList(bool extraParam, T[] list)
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

        #region not implemented
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="index"></param>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Index into list
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>T value</returns>
        T IList<T>.this[int index]
        {
            get { return _list[index]; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }
        #endregion

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
        public ImmutableList<T> Add(T value)
        {
            int listLength = _list.Length;
            T[] newArray = new T[listLength + 1];

            Array.Copy(_list, 0, newArray, 0, listLength);

            newArray[listLength] = value;

            return new ImmutableList<T>(false, newArray);
        }

        /// <summary>
        /// Add and IEnumerable to this list
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public ImmutableList<T> AddRange(IEnumerable<T> enumerable)
        {
            return AddRange(enumerable.ToArray());
        }

        /// <summary>
        /// Adds a range of T to immutable list creating a new one
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ImmutableList<T> AddRange(params T[] value)
        {
            int listLength = _list.Length;
            int valueLength = value.Length;
            T[] newArray = new T[listLength + valueLength];

            Array.Copy(_list, 0, newArray, 0, listLength);
            Array.Copy(value, 0, newArray, listLength, valueLength);

            return new ImmutableList<T>(false, newArray);
        }

    }
}
