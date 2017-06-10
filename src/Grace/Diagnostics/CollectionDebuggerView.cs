using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Debugger class for IEnumerables that need to be displayed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayValue) + ",nq}")]
    public class CollectionDebuggerView<T>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="items"></param>
        public CollectionDebuggerView(IEnumerable<T> items)
        {
            var array = items.ToArray();

            DebuggerDisplayValue = $"Count: {array.Length}";

            Items = array.Length > 0 ? (IEnumerable<T>)array : new List<T>();
        }

        /// <summary>
        /// Items
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IEnumerable<T> Items { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplayValue { get; }
    }
}
