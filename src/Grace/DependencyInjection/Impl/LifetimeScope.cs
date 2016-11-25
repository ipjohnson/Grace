using System;
using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Represents a scope that can be resolved from but doesn't allow exports to be registered in
    /// Note: This is the recommend scope for "per request" scenarios
    /// </summary>
    public class LifetimeScope : BaseExportLocatorScope, IExportLocatorScope
    {
        private readonly IInjectionScope _injectionScope;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="parent">parent for scope</param>
        /// <param name="name">name of scope</param>
        /// <param name="activationDelegates">activation delegate cache</param>
        public LifetimeScope(IExportLocatorScope parent, string name, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates) : base(parent, name, activationDelegates)
        {
            var currentScope = parent;

            while (currentScope != null && !(currentScope is IInjectionScope))
            {
                currentScope = currentScope.Parent;
            }

            _injectionScope = currentScope as IInjectionScope;

            if (_injectionScope == null)
            {
                throw new ArgumentException("Parent must have IInjectionScope", nameof(parent));
            }
        }

        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="consider"></param>
        /// <param name="key">key to use while locating</param>
        /// <returns></returns>
        public bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null)
        {
            return _injectionScope.CanLocate(type, consider, key);
        }

        /// <summary>
        /// Locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>located instance</returns>
        public object Locate(Type type)
        {
            var hashCode = type.GetHashCode();

            var func = ActivationDelegates[hashCode & ArrayLengthMinusOne].GetValueOrDefault(type, hashCode);

            return func != null ? func(this, this, null) : LocateFromParent(type, null, null, null, true, false);
        }

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data to be used during construction</param>
        /// <param name="consider"></param>
        /// <param name="withKey">key to use for locating type</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            return LocateFromParent(type, extraData, consider, withKey, true, isDynamic);
        }

        /// <summary>
        /// Create as a new IExportLocate scope
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns>new scope</returns>
        public IExportLocatorScope BeginLifetimeScope(string scopeName = "")
        {
            return new LifetimeScope(this, scopeName, ActivationDelegates);
        }

        /// <summary>
        /// Create injection context
        /// </summary>
        /// <param name="extraData">extra data</param>
        /// <returns></returns>
        public IInjectionContext CreateContext(object extraData = null)
        {
            return _injectionScope.CreateContext(extraData);
        }

        /// <summary>
        /// Locate type
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <returns>located instance</returns>
        public T Locate<T>()
        {
            return (T)Locate(typeof(T));
        }

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="extraData">extra data</param>
        /// <param name="consider">filter out different strategies</param>
        /// <param name="withKey">key to use during construction</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            return (T)LocateFromParent(typeof(T), extraData, consider, withKey, true, isDynamic);
        }

        /// <summary>
        /// Locate all instances of a specific type
        /// </summary>
        /// <param name="type">type ot locate</param>
        /// <param name="extraData">extra data to be used while locating</param>
        /// <param name="consider">strategy filter</param>
        /// <param name="comparer">comparer to use to sort collection</param>
        /// <returns></returns>
        public List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null)
        {
            var context = _injectionScope.CreateContext(extraData);

            return _injectionScope.InternalLocateAll(this, this, type, context, consider, comparer);
        }

        /// <summary>
        /// Locate all of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">type to locate, can be null</param>
        /// <param name="extraData">extra data to use during locate</param>
        /// <param name="consider">filter for strategies</param>
        /// <param name="comparer">comparer</param>
        /// <returns></returns>
        public List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null, IComparer<T> comparer = null)
        {
            return _injectionScope.InternalLocateAll(this, this, type ?? typeof(T), extraData, consider, comparer);
        }

        /// <summary>
        /// Try to locate an export by type
        /// </summary>
        /// <typeparam name="T">locate type</typeparam>
        /// <param name="value">out value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        public bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            var returnValue = false;
            var objectValue = LocateFromParent(typeof(T), extraData, consider, withKey, false, isDynamic);

            if (objectValue != null)
            {
                returnValue = true;
                value = (T)objectValue;
            }
            else
            {
                value = default(T);
            }

            return returnValue;
        }

        /// <summary>
        /// try to locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="value">located value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        public bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            value = LocateFromParent(type, extraData, consider, withKey, false, isDynamic);

            return value != null;
        }

        /// <summary>
        /// Locate from a parent scope if it's not in the cache
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data</param>
        /// <param name="consider">filter for strategies</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="isRequired">is it required</param>
        /// <param name="isDynamic">is the request dynamic</param>
        /// <returns></returns>
        protected virtual object LocateFromParent(Type type, object extraData, ActivationStrategyFilter consider, object key, bool isRequired, bool isDynamic)
        {
            return _injectionScope.LocateFromChildScope(this, this, type, extraData, consider, key, isRequired, isDynamic);
        }
    }
}
