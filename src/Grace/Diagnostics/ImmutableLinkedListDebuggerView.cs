using System.Diagnostics;
using System.Linq;
using Grace.Data.Immutable;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Class for debugger view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImmutableLinkedListDebugView<T>
    {
        private readonly ImmutableLinkedList<T> _immutableLinkedList;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="immutableLinkedList"></param>
        public ImmutableLinkedListDebugView(ImmutableLinkedList<T> immutableLinkedList)
        {
            _immutableLinkedList = immutableLinkedList;
        }

        /// <summary>
        /// Items in list
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _immutableLinkedList.ToArray();
    }
}
