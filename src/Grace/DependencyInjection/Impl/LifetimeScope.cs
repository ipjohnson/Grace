using System;
using System.Collections.Generic;
using System.Threading;
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
        /// <param name="injectionScope"></param>
        /// <param name="name">name of scope</param>
        /// <param name="activationDelegates">activation delegate cache</param>
        public LifetimeScope(IExportLocatorScope parent, IInjectionScope injectionScope, string name, ActivationStrategyDelegateCache activationDelegates) : base(parent, name, activationDelegates)
        {
            _injectionScope = injectionScope;
        }

        /// <summary>
        /// Create as a new IExportLocate scope
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns>new scope</returns>
        public override IExportLocatorScope BeginLifetimeScope(string scopeName = "")
        {
            return new LifetimeScope(this, _injectionScope, scopeName, DelegateCache);
        }

        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="consider"></param>
        /// <param name="key">key to use while locating</param>
        /// <returns></returns>
        public override bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null)
        {
            return _injectionScope.CanLocate(type, consider, key);
        }

        /// <summary>
        /// Create injection context
        /// </summary>
        /// <param name="extraData">extra data</param>
        /// <returns>injection context</returns>
        public override IInjectionContext CreateContext(object extraData = null)
        {
            return _injectionScope.CreateContext(extraData);
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
        public override object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            if (isDynamic || withKey != null || consider != null)
            {
                return LocateFromParent(type, extraData, consider, withKey, false, isDynamic);
            }

            return DelegateCache.ExecuteActivationStrategyDelegateWithContext(type, this, false, extraData != null ? CreateContext(extraData) : null);
        }

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="extraData">extra data</param>
        /// <param name="consider">filter out different strategies</param>
        /// <param name="withKey">key to use during construction</param>
        /// <param name="isDynamic">bypass the cache and look at all possible</param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public override T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            return (T)Locate(typeof(T), extraData, consider, withKey, isDynamic);
        }

        /// <summary>
        /// Locate all instances of a specific type
        /// </summary>
        /// <param name="type">type ot locate</param>
        /// <param name="extraData">extra data to be used while locating</param>
        /// <param name="consider">strategy filter</param>
        /// <param name="comparer">comparer to use to sort collection</param>
        /// <returns>list of objects</returns>
        public override List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null)
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
        /// <returns>list of all T</returns>
        public override List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null, IComparer<T> comparer = null)
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
        public override bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            object outValue;

            if (TryLocate(typeof(T), out outValue, extraData, consider, withKey, isDynamic))
            {
                value = (T)outValue;

                return true;
            }

            value = default(T);

            return false;
        }

        /// <summary>
        /// try to locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="value">located value</param>
        /// <param name="extraData">extra data to be used during locate</param>
        /// <param name="consider">filter to use during location</param>
        /// <param name="withKey">key to use during locate</param>
        /// <param name="isDynamic">is the request dynamic</param>
        /// <returns>true if export could be located</returns>
        public override bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false)
        {
            if (!isDynamic && withKey == null && consider == null)
            {
                var hashCode = type.GetHashCode();

                value = DelegateCache.ExecuteActivationStrategyDelegateWithContext(type, this, true, extraData == null ? null : CreateContext(extraData));

                return value != null;
            }

            value = LocateFromParent(type, extraData, consider, withKey, true, isDynamic);

            return value != null;
        }

        /// <summary>
        /// Locate by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <returns></returns>
        public override object LocateByName(string name, object extraData = null, ActivationStrategyFilter consider = null)
        {
            return _injectionScope.LocateByNameFromChildScope(this, this, name, extraData, consider, false);
        }

        /// <summary>
        /// Locate all by specific name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <returns></returns>
        public override List<object> LocateAllByName(string name, object extraData = null, ActivationStrategyFilter consider = null)
        {
            return _injectionScope.InternalLocateAllByName(this, this, name, extraData, consider);
        }

        /// <summary>
        /// Try to locate by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <returns></returns>
        public override bool TryLocateByName(string name, out object value, object extraData = null, ActivationStrategyFilter consider = null)
        {
            value = _injectionScope.LocateByNameFromChildScope(this, this, name, extraData, consider, true);

            return value != null;
        }

        /// <summary>
        /// Locate from a parent scope if it's not in the cache
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data</param>
        /// <param name="consider">filter for strategies</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="allowNull">is null allowed</param>
        /// <param name="isDynamic">is the request dynamic</param>
        /// <returns></returns>
        protected virtual object LocateFromParent(Type type, object extraData, ActivationStrategyFilter consider, object key, bool allowNull, bool isDynamic)
        {
            return _injectionScope.LocateFromChildScope(this, this, type, extraData, consider, key, allowNull, isDynamic);
        }

#if !NETSTANDARD1_0
        object IServiceProvider.GetService(Type type)
        {
            return DelegateCache.ExecuteActivationStrategyDelegateAllowNull(type, this);
        }
#endif
    }
}
