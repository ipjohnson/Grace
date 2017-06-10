using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Grace.Data.Immutable;
using Grace.Diagnostics;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Metadata for activation strategy
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayValue) + ",nq}")]
    [DebuggerTypeProxy(typeof(ActivationStrategyMetadataDebuggerView))]
    public class ActivationStrategyMetadata : IActivationStrategyMetadata
    {
        private readonly ImmutableHashTree<object, object> _metadata;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="exportAs"></param>
        /// <param name="exportAsKeyed"></param>
        /// <param name="metadata"></param>
        public ActivationStrategyMetadata(Type activationType, 
                                          IEnumerable<Type> exportAs, 
                                          IEnumerable<KeyValuePair<Type, object>> exportAsKeyed,
                                          ImmutableHashTree<object, object> metadata)
        {
            _metadata = metadata;
            ActivationType = activationType;
            ExportAs = exportAs;
            ExportAsKeyed = exportAsKeyed;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _metadata.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => _metadata.Count;

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        /// <param name="key">The key to locate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        public bool ContainsKey(object key)
        {
            return _metadata.ContainsKey(key);
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> interface contains an element that has the specified key; otherwise, false.</returns>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        public bool TryGetValue(object key, out object value)
        {
            return _metadata.TryGetValue(key, out value);
        }

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <param name="key">The key to locate.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found. </exception>
        public object this[object key] => _metadata[key];

        /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary. </summary>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        public IEnumerable<object> Keys => _metadata.Keys;

        /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        public IEnumerable<object> Values => _metadata.Values;

        /// <summary>
        /// Activation type
        /// </summary>
        public Type ActivationType { get; }

        /// <summary>
        /// Exported as 
        /// </summary>
        public IEnumerable<Type> ExportAs { get; }

        /// <summary>
        /// Exported as keyed
        /// </summary>
        public IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed { get; }

        /// <summary>
        /// Check to see if specific metadata matches
        /// </summary>
        /// <param name="key">key to use</param>
        /// <param name="value">value to compare</param>
        /// <returns></returns>
        public bool MetadataMatches(object key, object value)
        {
            object outValue;

            if (_metadata.TryGetValue(key, out outValue))
            {
                if (value != null)
                {
                    return value.Equals(outValue);
                }

                if (outValue == null)
                {
                    return true;
                }
            }
            else if (value == null)
            {
                return true;
            }

            return false;
        }

        private string DebuggerDisplayValue => $"Count: {_metadata.Count}";
    }
}
