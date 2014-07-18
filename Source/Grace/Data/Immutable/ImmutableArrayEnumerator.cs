using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Data.Immutable
{
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
            _count = -1;
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
