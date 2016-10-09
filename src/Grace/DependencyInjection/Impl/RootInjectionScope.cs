using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grace.Data.Immutable;
using System.Reflection;
using System.Threading;
using Grace.DependencyInjection.Impl.Wrappers;

namespace Grace.DependencyInjection.Impl
{
    public class RootInjectionScope : DisposalScope, IInjectionScope
    {
        #region Fields
        private string _scopeIdString;
        private Guid _scopeId = Guid.Empty;
        private readonly int _arraySizeMinusOne;
        private readonly ImmutableHashTree<Type, ActivationStrategyDelegate>[] _activationStrategyDelegates;
        private ImmutableHashTree<object, object> _extraData = ImmutableHashTree<object, object>.Empty;
        private ImmutableHashTree<string, object> _lockObjects = ImmutableHashTree<string, object>.Empty;
        private readonly ImmutableLinkedList<IMissingExportStrategyProvider> _missingExportStrategyProviders =
            ImmutableLinkedList<IMissingExportStrategyProvider>.Empty;
        private IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> _wrappers;
        protected IActivationStrategyCompiler ActivationStrategyCompiler;
        protected ILifetimeScopeProvider LifetimeScopeProvider;
        protected IInjectionContextCreator InjectionContextCreator;

        public const string ActivationStrategyAddLockName = "ActivationStrategyAddLock";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that takes configuration action
        /// </summary>
        /// <param name="configuration">configuration action</param>
        public RootInjectionScope(Action<InjectionScopeConfiguration> configuration) : this(CreateConfiguration(configuration))
        {

        }

        /// <summary>
        /// Configuration object constructor
        /// </summary>
        /// <param name="configuration"></param>
        public RootInjectionScope(IInjectionScopeConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Parent = null;
            Name = "RootScope";

            configuration.SetInjectionScope(this);

            ScopeConfiguration = configuration;

            InjectionContextCreator = configuration.Implementation.Locate<IInjectionContextCreator>();

            ActivationStrategyCompiler = configuration.Implementation.Locate<IActivationStrategyCompiler>();

            StrategyCollectionContainer =
                configuration.Implementation.Locate<IActivationStrategyCollectionContainer<ICompiledExportStrategy>>();

            DecoratorCollectionContainer =
                configuration.Implementation.Locate<IActivationStrategyCollectionContainer<ICompiledDecoratorStrategy>>();

            _arraySizeMinusOne = configuration.CacheArraySize - 1;
            _activationStrategyDelegates = new ImmutableHashTree<Type, ActivationStrategyDelegate>[configuration.CacheArraySize];

            for (var i = 0; i < configuration.CacheArraySize; i++)
            {
                _activationStrategyDelegates[i] = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
            }

            if (configuration.AutoRegisterUnknown)
            {
                _missingExportStrategyProviders =
                    _missingExportStrategyProviders.Add(
                        configuration.Implementation.Locate<IMissingExportStrategyProvider>());
            }
        }

        #endregion

        #region Public members

        /// <summary>
        /// Can Locator type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <param name="key">key to use while locating</param>
        /// <returns></returns>
        public bool CanLocate(Type type, object key = null)
        {
            if (key != null)
            {
                var collection = StrategyCollectionContainer.GetActivationStrategyCollection(type);

                return collection.GetKeyedStrategy(key) != null;
            }

            if (StrategyCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (WrapperCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (type.IsArray)
            {
                return true;
            }

            if (type.IsConstructedGenericType)
            {
                var generic = type.GetGenericTypeDefinition();

                if (StrategyCollectionContainer.GetActivationStrategyCollection(generic) != null)
                {
                    return true;
                }

                if (WrapperCollectionContainer.GetActivationStrategyCollection(generic) != null)
                {
                    return true;
                }

                if (generic == typeof(IEnumerable<>) ||
                    generic == typeof(IList<>) ||
                    generic == typeof(ICollection<>) ||
                    generic == typeof(IReadOnlyList<>) ||
                    generic == typeof(IReadOnlyCollection<>) ||
                    generic == typeof(List<>) ||
                    generic == typeof(ReadOnlyCollection<>) ||
                    generic == typeof(ImmutableLinkedList<>) ||
                    generic == typeof(ImmutableArray<>))
                {
                    return ScopeConfiguration.AutoRegisterUnknown;
                }
            }

            if (!type.GetTypeInfo().IsInterface)
            {
                return ScopeConfiguration.AutoRegisterUnknown;
            }

            return type == typeof(ILocatorService) ||
                   type == typeof(IExportLocatorScope) || 
                   type == typeof(IInjectionContext) ||
                   type == typeof(StaticInjectionContext);
        }

        /// <summary>
        /// Locate a specific type
        /// </summary>
        /// <param name="type">type to locate</param>
        /// <returns>located instance</returns>
        public object Locate(Type type)
        {
            var hashCode = type.GetHashCode();

            var func = _activationStrategyDelegates[hashCode & _arraySizeMinusOne].GetValueOrDefault(type, hashCode);

            return func != null ? func(this, this, null) : LocateObjectFactory(this, type, null, null, false);
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
        /// <param name="type">type to locate</param>
        /// <param name="extraData">extra data to be used during construction</param>
        /// <param name="withKey">key to use for locating type</param>
        /// <returns>located instance</returns>
        public object Locate(Type type, object extraData = null, object withKey = null)
        {
            if (withKey != null)
            {
                IInjectionContext context = CreateInjectionContextFromExtraData(type, extraData);

                return LocateObjectFactory(this, type, withKey, context, false);
            }
            else
            {
                var hash = type.GetHashCode();

                var func = _activationStrategyDelegates[hash & _arraySizeMinusOne].GetValueOrDefault(type, hash);

                IInjectionContext context = CreateInjectionContextFromExtraData(type, extraData);

                return func != null ? func(this, this, context) : LocateObjectFactory(this, type, null, context, false);
            }
        }

        /// <summary>
        /// Locate specific type using extra data or key
        /// </summary>
        /// <typeparam name="T">type to locate</typeparam>
        /// <param name="extraData">extra data</param>
        /// <param name="withKey">key to use during construction</param>
        /// <returns>located instance</returns>
        public T Locate<T>(object extraData = null, object withKey = null)
        {
            return (T)Locate(typeof(T), extraData, withKey);
        }

        public List<object> LocateAll(Type type, object extraData = null, ExportStrategyFilter filter = null, object withKey = null, IComparer<object> comparer = null)
        {
            return InternalLocateAll(type, extraData, filter, withKey, comparer);
        }

        public List<T> LocateAll<T>(object extraData = null, ExportStrategyFilter filter = null, object withKey = null, IComparer<T> comparer = null)
        {
            return InternalLocateAll(typeof(T), extraData, filter, withKey, comparer);
        }

        public bool TryLocate<T>(out T value, object extraData = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            IInjectionContext context = CreateInjectionContextFromExtraData(typeof(T), extraData);

            var newValue = LocateObjectFactory(this, typeof(T), withKey, context, true);

            bool returnValue = false;

            if (newValue != null)
            {
                returnValue = true;
                value = (T) newValue;
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
            throw new NotImplementedException();
        }


        /// <summary>
        /// Parent scope
        /// </summary>
        public IExportLocatorScope Parent { get; }

        /// <summary>
        /// Unique id for each scope
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

                _scopeId = new Guid(_scopeIdString);

                return _scopeId;
            }
        }

        /// <summary>
        /// Name of the scope
        /// </summary>
        public string Name { get; }

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
        /// Create as a new IExportLocate scope
        /// </summary>
        /// <param name="scopeName">scope name</param>
        /// <returns>new scope</returns>
        public IExportLocatorScope BeginLifetimeScope(string scopeName = "")
        {
            return LifetimeScopeProvider == null
                ? new LifetimeScope(this, scopeName, _activationStrategyDelegates)
                : LifetimeScopeProvider.CreateScope(this, scopeName, _activationStrategyDelegates);
        }

        /// <summary>
        /// Configure the injection scope
        /// </summary>
        /// <param name="registrationBlock"></param>
        public void Configure(Action<IExportRegistrationBlock> registrationBlock)
        {
            lock (GetLockObject(ActivationStrategyAddLockName))
            {
                var provider = ScopeConfiguration.Implementation.Locate<IExportRegistrationBlockValueProvider>();

                registrationBlock(provider);

                foreach (var inspector in provider.GetInspectors())
                {
                    StrategyCollectionContainer.AddInspector(inspector);
                    WrapperCollectionContainer.AddInspector(inspector);
                    DecoratorCollectionContainer.AddInspector(inspector);
                }

                foreach (var compiledWrapperStrategy in provider.GetWrapperStrategies())
                {
                    WrapperCollectionContainer.AddStrategy(compiledWrapperStrategy);
                }

                foreach (var decorator in provider.GetDecoratorStrategies())
                {
                    DecoratorCollectionContainer.AddStrategy(decorator);
                }

                foreach (var strategy in provider.GetExportStrategies())
                {
                    StrategyCollectionContainer.AddStrategy(strategy);

                    foreach (var secondaryStrategy in strategy.SecondaryStrategies())
                    {
                        StrategyCollectionContainer.AddStrategy(secondaryStrategy);
                    }
                }
            }
        }

        /// <summary>
        /// Scope configuration
        /// </summary>
        public IInjectionScopeConfiguration ScopeConfiguration { get; }

        /// <summary>
        /// Strategies associated with this scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledExportStrategy> StrategyCollectionContainer { get; }

        /// <summary>
        /// Wrappers associated with this scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> WrapperCollectionContainer => _wrappers ?? GetWrappers();


        /// <summary>
        /// Decorators associated with this scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledDecoratorStrategy> DecoratorCollectionContainer { get; }

        /// <summary>
        /// List of missing export strategy providers
        /// </summary>
        public IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders
            => _missingExportStrategyProviders;

        /// <summary>
        /// Locate an export from a child scope
        /// </summary>
        /// <param name="childScope">scope where the locate originated</param>
        /// <param name="type">type to locate</param>
        /// <param name="extraData"></param>
        /// <param name="key"></param>
        /// <param name="allowNull"></param>
        /// <returns>configuration object</returns>
        public object LocateFromChildScope(IExportLocatorScope childScope, Type type, object extraData, object key, bool allowNull)
        {
            return LocateObjectFactory(childScope, type, key, null, allowNull);
        }

        public object GetExtraData(object key)
        {
            return _extraData.GetValueOrDefault(key);
        }

        public void SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            ImmutableHashTree.ThreadSafeAdd(ref _extraData, key, newValue, replaceIfExists);
        }
        #endregion

        #region Non public members

        /// <summary>
        /// Creates a new configuration object
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IInjectionScopeConfiguration CreateConfiguration(Action<InjectionScopeConfiguration> configuration)
        {
            var configurationObject = new InjectionScopeConfiguration();

            configuration?.Invoke(configurationObject);

            return configurationObject;
        }


        protected virtual IInjectionContext CreateInjectionContextFromExtraData(Type type, object extraData)
        {
            return InjectionContextCreator.CreateContext(type, extraData);
        }

        private object LocateObjectFactory(IExportLocatorScope scope, Type type, object key, IInjectionContext injectionContext, bool allowNull)
        {
            var compiledDelegate = ActivationStrategyCompiler.FindDelegate(this, type, key);

            if (compiledDelegate != null)
            {
                if (key == null)
                {
                    compiledDelegate = AddObjectFactory(type, compiledDelegate);
                }

                return compiledDelegate(scope, scope, injectionContext);
            }

            if (!allowNull)
            {
                throw new Exception("Could not locate type: " + type.FullName);
            }

            return null;
        }

        private ActivationStrategyDelegate AddObjectFactory(Type type, ActivationStrategyDelegate activationStrategyDelegate)
        {
            var hashCode = type.GetHashCode();

            return ImmutableHashTree.ThreadSafeAdd(ref _activationStrategyDelegates[hashCode & _arraySizeMinusOne],
                                                   type,
                                                   activationStrategyDelegate);
        }

        private IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> GetWrappers()
        {
            if (_wrappers != null)
            {
                return _wrappers;
            }

            var wrapperCollectionProvider = ScopeConfiguration.Implementation.Locate<IDefaultWrapperCollectionProvider>();

            Interlocked.CompareExchange(ref _wrappers, wrapperCollectionProvider.ProvideCollection(this), null);

            return _wrappers;
        }

        private List<T> InternalLocateAll<T>(Type type, object extraData, ExportStrategyFilter filter, object withKey, IComparer<T> comparer)
        {
            List<T> returnList = new List<T>();

            LocateEnumerablesFromStrategyCollection(type, extraData, filter, returnList);

            if (type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                LocateEnumerablesFromStrategyCollection(genericType, extraData, filter, returnList);
            }

            if (comparer != null)
            {
                returnList.Sort(comparer);
            }

            return returnList;
        }

        private void LocateEnumerablesFromStrategyCollection<T>(Type type, object extraData, ExportStrategyFilter filter,
            List<T> returnList)
        {
            var collection = StrategyCollectionContainer.GetActivationStrategyCollection(type);

            if (collection != null)
            {
                foreach (var strategy in collection.GetStrategies())
                {
                    if (filter != null && !filter(strategy))
                    {
                        continue;
                    }

                    if (strategy.HasConditions)
                    {
                        throw new NotImplementedException();
                    }

                    var activationDelegate = strategy.GetActivationStrategyDelegate(this, ActivationStrategyCompiler, type);

                    if (activationDelegate != null)
                    {
                        returnList.Add(
                            (T)
                            activationDelegate(this, this,
                                extraData != null ? CreateInjectionContextFromExtraData(type, extraData) : null));
                    }
                }
            }
        }

        #endregion

    }
}
