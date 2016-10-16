using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class LifetimeScope : BaseExportLocatorScope, IExportLocatorScope
    {
        private readonly IInjectionScope _injectionScope;

        public LifetimeScope(IExportLocatorScope parent, string name, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates) : base(parent,name,activationDelegates)
        {
            var currentScope = parent;

            while (!(currentScope is IInjectionScope))
            {
                currentScope = currentScope.Parent;
            }

            _injectionScope = currentScope as IInjectionScope;
        }

        public bool CanLocate(Type type, object key = null)
        {
            return _injectionScope.CanLocate(type);
        }

        public object Locate(Type type)
        {
            var hashCode = type.GetHashCode();

            var func = ActivationDelegates[hashCode & ArrayLengthMinusOne].GetValueOrDefault(type, hashCode);

            return func != null ? func(this, this, null) : LocateFromParent(type, null, null,true);
        }
        
        public object Locate(Type type, object extraData = null, object key = null, bool isDynamic = false)
        {
            return LocateFromParent(type, extraData, key, true);
        }
        
        public IExportLocatorScope BeginLifetimeScope(string scopeName = "")
        {
            return new LifetimeScope(this, scopeName, ActivationDelegates);
        }

        public T Locate<T>()
        {
            return (T)Locate(typeof(T));
        }

        public T Locate<T>(object extraData = null, object withKey = null, bool isDynamic = false)
        {
            return (T)LocateFromParent(typeof(T), extraData, withKey, true);
        }

        public List<object> LocateAll(Type type, object extraData = null, ExportStrategyFilter filter = null, object withKey = null,
            IComparer<object> comparer = null)
        {
            return Parent.LocateAll(type, extraData, filter, withKey, comparer);
        }

        public List<T> LocateAll<T>(object extraData = null, ExportStrategyFilter filter = null, object withKey = null,
            IComparer<T> comparer = null)
        {
            return Parent.LocateAll<T>(extraData, filter, withKey, comparer);
        }

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

        public bool TryLocate(Type type, out object value, object extraData = null, ExportStrategyFilter consider = null,
            object withKey = null)
        {
            value = LocateFromParent(type, extraData, withKey, false);

            return value != null;
        }

        protected virtual object LocateFromParent(Type type, object extraData, object key, bool isRequired)
        {
            return _injectionScope.LocateFromChildScope(this, type, extraData, key, isRequired);
        }
    }
}
