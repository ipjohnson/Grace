using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// base locator scope used by InjectionScope and LifetimeScope
    /// </summary>
    public abstract partial class BaseExportLocatorScope : DisposalScope, IExportLocatorScope
    {
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;
        
        /// <summary>
        /// Cache delegate for base scopes
        /// </summary>
        protected readonly ActivationStrategyDelegateCache DelegateCache;

        /// <summary>
        /// Internal scoped storage used by Scoped Lifestyle
        /// </summary>
        protected ScopedStorage InternalScopedStorage = ScopedStorage.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parent">parent scope</param>
        /// <param name="name">name of scope</param>
        /// <param name="delegateCache">delegate cache</param>
        protected BaseExportLocatorScope(IExportLocatorScope parent,
                                      string name,
                                      ActivationStrategyDelegateCache delegateCache)
        {
            Parent = parent;
            ScopeName = name ?? "";

            DelegateCache = delegateCache;
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
        public Guid ScopeId =>
            (Guid)( GetExtraData("LocatorScopeId") ??
                    SetExtraData("LocatorScopeId", Guid.NewGuid(), false));

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

        /// <summary>
        /// Locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>located instance</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Locate(Type type)
        {
            return DelegateCache.ExecuteActivationStrategyDelegate(type, this);
        }

        /// <summary>
        /// Locate type or return default value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object LocateOrDefault(Type type, object defaultValue)
        {
            return DelegateCache.ExecuteActivationStrategyDelegateAllowNull(type, this) ?? defaultValue;
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
        /// Locate or return default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T LocateOrDefault<T>(T defaultValue = default(T))
        {
            return (T)LocateOrDefault(typeof(T), defaultValue);
        }

        public T GetOrCreateScopedService<T>(int id, ActivationStrategyDelegate createDelegate, IInjectionContext context)
        {
            var initialStorage = InternalScopedStorage;
            var storage = initialStorage;

            while (!ReferenceEquals(storage, ScopedStorage.Empty))
            {
                if (storage.Id == id)
                {
                    return (T)storage.ScopedService;
                }

                storage = storage.Next;
            }

            var value = createDelegate(this, this, context);

            if (Interlocked.CompareExchange(ref InternalScopedStorage, new ScopedStorage { Id = id, Next = initialStorage, ScopedService = value }, initialStorage) == initialStorage)
            {
                return (T)value;
            }

            return HandleScopedStorageCollision<T>(id, (T)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="createDelegate"></param>
        /// <returns></returns>
        public virtual T GetOrCreateScopedService<T>(int id, Func<T> createDelegate)
        {
            var initialStorage = InternalScopedStorage;
            var storage = initialStorage;

            while (!ReferenceEquals(storage, ScopedStorage.Empty))
            {
                if (storage.Id == id)
                {
                    return (T)storage.ScopedService;
                }

                storage = storage.Next;
            }

            var value = createDelegate();

            if (ReferenceEquals(
                Interlocked.CompareExchange(ref InternalScopedStorage, new ScopedStorage { Id = id, Next = initialStorage, ScopedService = value }, initialStorage), 
                initialStorage))
            {
                return (T)value;
            }

            return HandleScopedStorageCollision<T>(id, (T)value);

        }


        private T HandleScopedStorageCollision<T>(int id, T value)
        {
            ScopedStorage newStorage = new ScopedStorage { Id = id, ScopedService = value };

            SpinWait spinWait = new SpinWait();
            var initialStorage = InternalScopedStorage;

            do
            {
                var current = initialStorage;
                newStorage.Next = current;

                while (!ReferenceEquals(current, ScopedStorage.Empty))
                {
                    if (current.Id == id)
                    {
                        return value;
                    }

                    current = current.Next;
                }

                if (ReferenceEquals(
                        Interlocked.CompareExchange(ref InternalScopedStorage, newStorage, initialStorage),
                        initialStorage))
                {
                    return value;
                }

                spinWait.SpinOnce();
                initialStorage = InternalScopedStorage;
            }
            while (true);
        }


#if !NETSTANDARD1_0
        object IServiceProvider.GetService(Type type)
        {
            return DelegateCache.ExecuteActivationStrategyDelegateAllowNull(type, this);
        }
#endif

        public abstract IExportLocatorScope BeginLifetimeScope(string scopeName = "");

        public abstract IInjectionContext CreateContext(object extraData = null);

        
        public abstract object Locate(Type type, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
        public abstract T Locate<T>(object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
        public abstract List<object> LocateAll(Type type, object extraData = null, ActivationStrategyFilter consider = null, IComparer<object> comparer = null);
        public abstract List<T> LocateAll<T>(Type type = null, object extraData = null, ActivationStrategyFilter consider = null, IComparer<T> comparer = null);
        public abstract bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        public abstract bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);

        public abstract object LocateByName(string name, object extraData = null, ActivationStrategyFilter consider = null);

        public abstract List<object> LocateAllByName(string name, object extraData = null, ActivationStrategyFilter consider = null);

        public abstract bool TryLocateByName(string name, out object value, object extraData = null, ActivationStrategyFilter consider = null);

        public abstract bool CanLocate(Type type, ActivationStrategyFilter consider = null, object key = null);


        /// <summary>
        /// class for storing scoped instances
        /// </summary>
        protected class ScopedStorage
        {
            /// <summary>
            /// storage id
            /// </summary>
            public int Id;

            /// <summary>
            /// Scoped service instance
            /// </summary>
            public object ScopedService;

            /// <summary>
            /// Next scoped storage in list
            /// </summary>
            public ScopedStorage Next;

            /// <summary>
            /// 
            /// </summary>
            public static ScopedStorage Empty;

            static ScopedStorage()
            {
                var empty = new ScopedStorage();
                empty.Next = empty; // this is intentional and need so you can make the assumption that Next is never null
                Empty = empty;
            }
        }
    }
}
