using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class LifetimeScope : DisposalScope, IExportLocatorScope
    {
        private string _scopeIdString;
        private Guid _scopeId = Guid.Empty;
        private readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] _activationDelegates;
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;
        private readonly int _arrayLengthMinusOne;
        private readonly IInjectionScope _injectionScope;

        public LifetimeScope(IExportLocatorScope parent, string name, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates)
        {
            Parent = parent;
            Name = name ?? "";
            _activationDelegates = activationDelegates;
            _arrayLengthMinusOne = activationDelegates.Length - 1;

            var currentScope = parent;

            while (!(currentScope is IInjectionScope))
            {
                currentScope = currentScope.Parent;
            }

            _injectionScope = currentScope as IInjectionScope;
        }

        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }

        public void SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, true);
        }

        public IExportLocatorScope Parent { get; }

        public string Name { get; }

        public Guid ScopeId
        {
            get
            {
                if (_scopeId != Guid.Empty)
                {
                    return _scopeId;
                }

                Interlocked.CompareExchange(ref _scopeIdString, Guid.NewGuid().ToString(), null);

                _scopeId = new Guid(_scopeIdString);

                return _scopeId;
            }
        }

        public bool CanLocate(Type type, object key = null)
        {
            return _injectionScope.CanLocate(type);
        }

        public object Locate(Type type)
        {
            var hashCode = type.GetHashCode();

            var func = _activationDelegates[hashCode & _arrayLengthMinusOne].GetValueOrDefault(type, hashCode);

            return func != null ? func(this, this, null) : LocateFromParent(type, null, null,true);
        }


        public object Locate(Type type, object extraData = null, object key = null)
        {
            return LocateFromParent(type, extraData, key, true);
        }

        public object GetLockObject(string lockName)
        {
            return _lockObjects.GetValueOrDefault(lockName) ??
                   ImmutableHashTree.ThreadSafeAdd(ref _lockObjects, lockName, new object());
        }

        public IExportLocatorScope BeginLifetimeScope(string scopeName = "")
        {
            return new LifetimeScope(this, scopeName, _activationDelegates);
        }

        public T Locate<T>()
        {
            return (T)Locate(typeof(T));
        }

        public T Locate<T>(object extraData = null, object withKey = null)
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
