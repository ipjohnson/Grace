using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Export metadata 
    /// </summary>
    public class ExportMetadata : IExportMetadata
    {
        private readonly object _lockObject = new object();
        private ImmutableHashTree<string, object> _metadata = ImmutableHashTree<string, object>.Empty;
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dictionary"></param>
        public ExportMetadata(object key, IEnumerable<KeyValuePair<string, object>> dictionary = null)
        {
            Key = key;

            if (dictionary != null)
            {
                foreach (KeyValuePair<string, object> keyValuePair in dictionary)
                {
                    _metadata = _metadata.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        /// <summary>
        /// Key the export was registered with
        /// </summary>
        public object Key { get; private set; }

        /// <summary>
        /// Returns true if there is no metadata.
        /// Note: this method is recommended over if(Count > 0)
        /// </summary>
        public bool IsEmpty
        {
            get { return _metadata.IsEmpty; }
        }

        /// <summary>
        /// Tests to see if metadata values match
        /// </summary>
        /// <param name="metadataName"></param>
        /// <param name="metadataValue"></param>
        /// <returns></returns>
        public bool MetadataMatches(string metadataName, object metadataValue)
        {
            object testValue;

            if (metadataValue != null)
            {
                return TryGetValue(metadataName, out testValue) &&
                       metadataValue.Equals(testValue);
            }

            return TryGetValue(metadataName, out testValue) &&
                   testValue == null;
        }

        /// <summary>
        /// Adds or updates a metadata value. 
        /// </summary>
        /// <param name="metadataName">metadata name</param>
        /// <param name="metadataValue">metadata value</param>
        public void AddOrUpdate(string metadataName, object metadataValue)
        {
            lock (_lockObject)
            {
                _metadata = _metadata.Add(metadataName, metadataValue, (x, y) => metadataValue);
            }
        }

        /// <summary>
        /// Gets an enumerator for the metadata
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _metadata.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the metadata
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a count of the metadata
        /// </summary>
        public int Count
        {
            get { return _metadata.Count(); }
        }

        /// <summary>
        /// Returns true if there is a metadata value by the specified name
        /// </summary>
        /// <param name="key">metadata key</param>
        /// <returns>true if key exists</returns>
        public bool ContainsKey(string key)
        {
            return _metadata.ContainsKey(key);
        }

        /// <summary>
        /// Attemps to fetch the metadata value by the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value)
        {
            return _metadata.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns the metadata value by the specified key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>metadata value</returns>
        public object this[string key]
        {
            get { return _metadata[key]; }
        }

        /// <summary>
        /// Metadata keys for this export
        /// </summary>
        public IEnumerable<string> Keys
        {
            get { return _metadata.Keys; }
        }

        /// <summary>
        /// Metadata values for this export
        /// </summary>
        public IEnumerable<object> Values
        {
            get { return _metadata.Values; }
        }
    }
}