using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Data context that is shared between context in the same graph
    /// </summary>
    public class InjectionContextSharedData : IInjectionContextSharedData
    {
        private ImmutableHashTree<object,object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _locks = ImmutableHashTree<string, object>.Empty;

        /// <summary>
        /// Keys for data
        /// </summary>
        public IEnumerable<object> Keys => _extraData.Keys;

        /// <summary>
        /// Values for data
        /// </summary>
        public IEnumerable<object> Values => _extraData.Values;


        /// <summary>
        /// Enumeration of all the key value pairs
        /// </summary>
        public IEnumerable<KeyValuePair<object, object>> KeyValuePairs => _extraData;

        /// <summary>
        /// Extra data associated with the injection request. 
        /// </summary>
        /// <param name="key">key of the data object to get</param>
        /// <returns>data value</returns>
        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }
        
        /// <summary>
        /// Sets extra data on the injection context
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="newValue">new object value</param>
        /// <param name="replaceIfExists"></param>
        public object SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            return ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }

        /// <summary>
        /// Get a lock object by a specific name
        /// </summary>
        /// <param name="lockName"></param>
        /// <returns></returns>
        public object GetLockObject(string lockName)
        {
            return _locks.GetValueOrDefault(lockName) ??
                ImmutableHashTree.ThreadSafeAdd(ref _locks, lockName, new object());
        }
    }
}
