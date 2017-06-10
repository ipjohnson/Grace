using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grace.Data.Immutable;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Class for debugger
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ImmutableHashTreeDebuggerView<TKey, TValue>
    {
        private readonly ImmutableHashTree<TKey, TValue> _hashTree;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="hashTree"></param>
        public ImmutableHashTreeDebuggerView(ImmutableHashTree<TKey, TValue> hashTree)
        {
            _hashTree = hashTree;
        }

        /// <summary>
        /// Keys for hash tree
        /// </summary>
        public IEnumerable<TKey> Keys => _hashTree.Keys.ToList();

        /// <summary>
        /// Values
        /// </summary>
        public IEnumerable<TValue> Values => _hashTree.Values.ToList();

        /// <summary>
        /// Items
        /// </summary>
        public IEnumerable<KeyValuePairDebuggerView<TKey, TValue>> Items
        {
            get
            {
                var list =
                    _hashTree.Select(kvp => new KeyValuePairDebuggerView<TKey, TValue>(kvp.Key, kvp.Value)).ToList();

                list.Sort((x, y) => string.Compare(x.Key?.ToString(), y.Key?.ToString(), StringComparison.CurrentCultureIgnoreCase));

                return list;
            }
        }
    }

    /// <summary>
    /// Debugger view for hash tree entries
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayString) + ",nq}", Name = "{DebuggerNameDisplayString,nq}")]
    public class KeyValuePairDebuggerView<TKey, TValue>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public KeyValuePairDebuggerView(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Key for entry
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Value for entry
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Debugger display string
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayString => Value?.ToString();

        /// <summary>
        /// Debugger value string
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerNameDisplayString => Key?.ToString();
    }
}
