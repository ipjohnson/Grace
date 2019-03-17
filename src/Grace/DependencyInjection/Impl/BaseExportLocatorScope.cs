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
    public abstract class BaseExportLocatorScope : DisposalScope, IExtraDataContainer, IExportLocatorScope
    {
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;
        protected const int ActivationDelegatesLengthMinusOne = 63;
        private string _scopeIdString;
        private Guid _scopeId = Guid.Empty;
        
        /// <summary>
        /// array of activation delegates
        /// </summary>
        protected readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] ActivationDelegates;


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent">parent scope</param>
        /// <param name="name">name of scope</param>
        /// <param name="activationDelegates"></param>
        protected BaseExportLocatorScope(IExportLocatorScope parent,
                                      string name, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates)
        {
            Parent = parent;
            
            ScopeName = name ?? "";
            ActivationDelegates = activationDelegates;
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
        
        /// <inheritdoc />
        public object GetService(Type type)
        {
            var hashCode = type.GetHashCode();
            var currentNode = ActivationDelegates[hashCode & ActivationDelegatesLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value(this, this, null);
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return ReferenceEquals(currentNode.Key, type) ?
                currentNode.Value(this, this, null) :
                FallbackExecution(currentNode, type, true, null);
        }

        /// <inheritdoc />
        public abstract bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null);

        /// <inheritdoc />
        public virtual object Locate(Type type)
        {
            var hashCode = type.GetHashCode();
            var currentNode = ActivationDelegates[hashCode & ActivationDelegatesLengthMinusOne];

            if (ReferenceEquals(currentNode.Key, type))
            {
                return currentNode.Value(this, this, null);
            }

            while (currentNode.Hash != hashCode && currentNode.Height != 0)
            {
                currentNode = hashCode < currentNode.Hash ? currentNode.Left : currentNode.Right;
            }

            return ReferenceEquals(currentNode.Key, type) ?
                currentNode.Value(this, this, null) :
                FallbackExecution(currentNode, type, false, null);
        }

        /// <inheritdoc />
        public abstract object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        /// <inheritdoc />
        public object LocateOrDefault(Type type, object defaultValue) => GetService(type) ?? defaultValue;

        /// <inheritdoc />
        public T Locate<T>() => (T)Locate(typeof(T));

        /// <inheritdoc />
        public T LocateOrDefault<T>(T defaultValue = default(T)) => (T)(GetService(typeof(T)) ?? defaultValue);

        /// <inheritdoc />
        public T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null,object withKey = null,bool isDynamic = false) => 
            (T) Locate(typeof(T),extraData, consider, withKey, isDynamic);

        /// <inheritdoc />
        public abstract List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null);

        /// <inheritdoc />
        public abstract List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null,
            IComparer<T> comparer = null);

        /// <inheritdoc />
        public bool TryLocate<T>(out T value, object extraData = null,
            ActivationStrategyFilter consider = null, object withKey = null,
            bool isDynamic = false)
        {
            var returnValue = TryLocate(typeof(T), out var oValue, extraData, consider, withKey, isDynamic);

            value = (T) oValue;

            return returnValue;
        }

        /// <inheritdoc />
        public abstract bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null,
            object withKey = null, bool isDynamic = false);

        /// <inheritdoc />
        public abstract object LocateByName(string name, object extraData = null, ActivationStrategyFilter consider = null);

        /// <inheritdoc />
        public abstract List<object> LocateAllByName(string name, object extraData = null, ActivationStrategyFilter consider = null);
        
        /// <inheritdoc />
        public abstract bool TryLocateByName(string name, out object value, object extraData = null, ActivationStrategyFilter consider = null);

        /// <inheritdoc />
        public abstract IExportLocatorScope BeginLifetimeScope(string scopeName = "");

        /// <inheritdoc />
        public abstract IInjectionContext CreateContext(object extraData = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="type"></param>
        /// <param name="allowNull"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected object FallbackExecution(ImmutableHashTree<Type, ActivationStrategyDelegate> currentNode, Type type, bool allowNull, IInjectionContext context)
        {
            if (currentNode.Height != 0)
            {
                foreach (var kvp in currentNode.Conflicts)
                {
                    if (ReferenceEquals(kvp.Key, type))
                    {
                        return kvp.Value(this, this, context);
                    }
                }
            }

            var injectionScope = this.GetInjectionScope();

            return injectionScope.LocateFromChildScope(this, this, type, context, null, null, allowNull, false);
        }
    }
}
