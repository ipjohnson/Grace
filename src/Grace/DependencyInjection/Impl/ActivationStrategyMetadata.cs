using System;
using System.Collections;
using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class ActivationStrategyMetadata : IActivationStrategyMetadata
    {
        private readonly ImmutableHashTree<object, object> _metadata;

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

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _metadata.Count;

        public bool ContainsKey(object key)
        {
            return _metadata.ContainsKey(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _metadata.TryGetValue(key, out value);
        }

        public object this[object key] => _metadata[key];

        public IEnumerable<object> Keys => _metadata.Keys;

        public IEnumerable<object> Values => _metadata.Values;

        public Type ActivationType { get; }

        public IEnumerable<Type> ExportAs { get; }

        public IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed { get; }

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
    }
}
