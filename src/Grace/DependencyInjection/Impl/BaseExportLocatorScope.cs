using System;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class BaseExportLocatorScope : DisposalScope, IExtraDataContainer
    {
        private string _scopeIdString;
        private Guid _scopeId = Guid.Empty;
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;

        protected readonly int ArrayLengthMinusOne;
        protected readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] ActivationDelegates;

        public BaseExportLocatorScope(IExportLocatorScope parent, 
                                      string name, 
                                      ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationDelegates)
        {
            ActivationDelegates = activationDelegates;
            Parent = parent;
            Name = name;

            ActivationDelegates = activationDelegates;
            ArrayLengthMinusOne = activationDelegates.Length - 1;
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
        
        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }

        public void SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }

        public object GetLockObject(string lockName)
        {
            return _lockObjects.GetValueOrDefault(lockName) ??
                   ImmutableHashTree.ThreadSafeAdd(ref _lockObjects, lockName, new object());
        }

    }
}
