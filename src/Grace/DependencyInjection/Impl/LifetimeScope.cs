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
        public LifetimeScope(IExportLocatorScope parent, string name, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates) : base(parent,name,activationDelegates)
        {
            var currentScope = parent;

            while (!(currentScope is IInjectionScope))
            {
                currentScope = currentScope.Parent;
            }

            _injectionScope = currentScope as IInjectionScope;
        }

        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="key">key to use while locating</param>
        /// <returns></returns>
        public bool CanLocate(Type type, object key = null)
        {
            return _injectionScope.CanLocate(type);
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

            return func != null ? func(this, this, null) : LocateFromParent(type, null, null,true);
        }

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data to be used during construction</param>
        /// <param name="withKey">key to use for locating type</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public object Locate(Type type, object extraData = null, object withKey = null, bool isDynamic = false)
        {
            return LocateFromParent(type, extraData, withKey, true);
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
        /// <param name="withKey">key to use during construction</param>
        /// <param name="isDynamic"></param>
        /// <returns>located instance</returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public T Locate<T>(object extraData = null, object withKey = null, bool isDynamic = false)
        {
            return (T)LocateFromParent(typeof(T), extraData, withKey, true);
        }

        /// <summary>
        /// Locate all instances of a specific type
        /// </summary>
        /// <param name="type">type ot locate</param>
        /// <param name="extraData">extra data to be used while locating</param>
        /// <param name="filter">strategy filter</param>
        /// <param name="withKey">locate with key</param>
        /// <param name="comparer">comparer to use to sort collection</param>
        /// <returns></returns>
        public List<object> LocateAll(Type type, object extraData = null, ExportStrategyFilter filter = null, object withKey = null,
            IComparer<object> comparer = null)
        {
            return Parent.LocateAll(type, extraData, filter, withKey, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extraData"></param>
        /// <param name="filter"></param>
        /// <param name="withKey"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public List<T> LocateAll<T>(object extraData = null, ExportStrategyFilter filter = null, object withKey = null,
            IComparer<T> comparer = null)
        {
            return Parent.LocateAll<T>(extraData, filter, withKey, comparer);
        }

        /// <summary>
        /// Try to locate an export by type
        /// </summary>
        /// <typeparam name="T">locate type</typeparam>
        /// <param name="value">out value</param>
        /// <param name="extraData"></param>
        /// <param name="consider"></param>
        /// <param name="withKey"></param>
        /// <returns></returns>
        public bool TryLocate<T>(out T value, object extraData = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            var returnValue = false;
            var objectValue = LocateFromParent(typeof(T), extraData, withKey, false);

            if (objectValue != null)
            {
                returnValue = true;
                value = (T) objectValue;
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
        /// <returns></returns>
        public bool TryLocate(Type type, out object value, object extraData = null, ExportStrategyFilter consider = null,
            object withKey = null)
        {
            value = LocateFromParent(type, extraData, withKey, false);

            return value != null;
        }

        protected virtual object LocateFromParent(Type type, object extraData, object key, bool isRequired)
        {
            return _injectionScope.LocateFromChildScope(this, this, type, extraData, key, isRequired);
        }
    }
}
