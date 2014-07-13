using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data
{

    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ImmutableList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Empty immutable list
        /// </summary>
        public static readonly ImmutableList<T> Empty = new ImmutableList<T>(new T[0]);
        /// <summary>
        /// List of items
        /// </summary>
        public readonly T[] Items;

        /// <summary>
        /// Cosntructor takes an IEnumerable of T
        /// </summary>
        /// <param name="list"></param>
        public ImmutableList(IEnumerable<T> list)
        {
            Items = list.ToArray();
        }

        /// <summary>
        /// Constructor takes an array of T
        /// </summary>
        /// <param name="list"></param>
        public ImmutableList(T[] list)
        {
            Items = list;
        }

        /// <summary>
        /// Get an enumerator for the list
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Get an enumerator for the list
        /// </summary>
        /// <returns>enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a new immutable list with the specified new t
        /// </summary>
        /// <param name="newT">new instance</param>
        /// <returns>new list</returns>
        public ImmutableList<T> Add(T newT)
        {
            T[] newArray = new T[Items.Length + 1];

            Array.Copy(Items, 0, newArray, 0, Items.Length);

            return new ImmutableList<T>(newArray);
        }

        /// <summary>
        /// Enumerator class for immutable list
        /// </summary>
        private class Enumerator : IEnumerator<T>
        {
            private readonly ImmutableList<T> _immutableList;
            private int _iteration = -1;

            public Enumerator(ImmutableList<T> immutableList)
            {
                _immutableList = immutableList;
            }

            public bool MoveNext()
            {
                if (_iteration + 1 > _immutableList.Items.Length)
                {
                    _iteration++;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _iteration = -1;
            }

            public T Current
            {
                get { return _immutableList.Items[_iteration]; }
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
