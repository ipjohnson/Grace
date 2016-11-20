using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Debugger class for IEnumerables that need to be displayed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}")]
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
