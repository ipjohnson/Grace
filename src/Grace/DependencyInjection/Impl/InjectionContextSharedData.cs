using System;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class InjectionContextSharedData : IInjectionContextSharedData
    {
        private ImmutableHashTree<object,object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _locks = ImmutableHashTree<string, object>.Empty;

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
            return _locks.GetValueOrDefault(lockName) ??
                ImmutableHashTree.ThreadSafeAdd(ref _locks, lockName, new object());
        }
    }
}
