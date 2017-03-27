using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// base locator scope used by InjectionScope and LifetimeScope
    /// </summary>
    public abstract class BaseExportLocatorScope : DisposalScope, IExtraDataContainer
    {
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;

        private string _scopeIdString;
        private Guid _scopeId = Guid.Empty;
        
        /// <summary>
        /// length of the activation delegates array minus one
        /// </summary>
        protected readonly int ArrayLengthMinusOne;

        /// <summary>
        /// array of activation delegates
        /// </summary>
        protected readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] ActivationDelegates;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent">parent scope</param>
        /// <param name="name">name of scope</param>
        /// <param name="activationDelegates">activation delegates</param>
        protected BaseExportLocatorScope(IExportLocatorScope parent,
                                      string name,
                                      ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates)
        {
            Parent = parent;
            ScopeName = name ?? "";

            ActivationDelegates = activationDelegates;
            ArrayLengthMinusOne = activationDelegates.Length - 1;
        }

        /// <summary>
        /// Parent for scope
        /// </summary>
        public IExportLocatorScope Parent { get; }

        /// <summary>
        /// Name of scope
        /// </summary>
        public string ScopeName { get; }

        /// <summary>
        /// Scope id
        /// </summary>
        public Guid ScopeId
        {
            get
            {
                if (_scopeId != Guid.Empty)
                {
                    return _scopeId;
                }

                Interlocked.CompareExchange(ref _scopeIdString, Guid.NewGuid().ToString(), null);

                return _scopeId = new Guid(_scopeIdString);
            }
        }

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
        /// Sets extra data on the locator scope
        /// </summary>
        /// <param name="key">object name</param>
        /// <param name="newValue">new object value</param>
        /// <param name="replaceIfExists"></param>
        /// <returns>the final value of key</returns>
        public object SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            return ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }

        /// <summary>
        /// Gets a named object that can be used for locking
        /// </summary>
        /// <param name="lockName">lock name</param>
        /// <returns>lock</returns>
        public object GetLockObject(string lockName)
        {
            return _lockObjects.GetValueOrDefault(lockName) ??
                   ImmutableHashTree.ThreadSafeAdd(ref _lockObjects, lockName, new object());
        }
    }
}
