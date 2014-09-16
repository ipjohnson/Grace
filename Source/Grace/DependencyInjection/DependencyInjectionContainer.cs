using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;
using Grace.Diagnostics;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// This is the standard IDependencyInjectionContainer implementation
    /// </summary>
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    [DebuggerTypeProxy(typeof(DependencyInjectionContainerDiagnostic))]
    public class DependencyInjectionContainer : IDependencyInjectionContainer, IMissingExportHandler
    {
        private readonly BlackList _blackList = new BlackList();
        private readonly InjectionKernelManager _injectionKernelManager;
        protected bool _disposed;
        protected ImmutableArray<IMissingExportStrategyProvider> _missingExportStrategyProviders =
            ImmutableArray<IMissingExportStrategyProvider>.Empty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="environment">Environment that you want this container to operate in (RunTime, DesignTime, Or UnitTest</param>
        /// <param name="comparer">delegate that can be used to sort exports, if null is provided CompareExportStrategies will be used</param>
        /// <param name="disposalScopeProvider">allows you to provide a custom disposal scope provider</param>
        public DependencyInjectionContainer(ExportEnvironment environment = ExportEnvironment.RunTime,
            ExportStrategyComparer comparer = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            if (environment != ExportEnvironment.RunTime &&
                 environment != ExportEnvironment.DesignTime &&
                 environment != ExportEnvironment.UnitTest)
            {
                throw new ArgumentException(
                    "Environment must be one of RunTime, DesignTime, or UnitTest, all others are invalid for this purpose",
                    "environment");
            }

            ExportStrategyComparer localComparer = comparer ?? CompareExportStrategies;

            _injectionKernelManager = new InjectionKernelManager(this, localComparer, _blackList);

            AutoRegisterUnknown = true;
            ThrowExceptions = true;

            RootScope = new InjectionKernel(_injectionKernelManager, null, disposalScopeProvider, "RootScope", localComparer)
                            {
                                Environment = environment
                            };
        }

        /// <summary>
        /// If a concrete type is requested and it is not registered an export strategy will be created.
        /// Note: It will be scanned for attributes
        /// </summary>
        public bool AutoRegisterUnknown { get; set; }

        /// <summary>
        /// If true exception will be thrown if a type can't be located, otherwise it will be caught and errors logged
        /// False by default
        /// </summary>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        /// The root scope for the container
        /// </summary>
        [NotNull]
        public IInjectionScope RootScope { get; private set; }

        /// <summary>
        /// Black lists a particular export (Fullname)
        /// </summary>
        /// <param name="exportType">full name of the type to black list</param>
        public void BlackListExport(string exportType)
        {
            _blackList.Add(exportType);
        }

        /// <summary>
        /// Black list a particular export by Type
        /// </summary>
        /// <param name="exportType">type to black list</param>
        public void BlackListExportType(Type exportType)
        {
            _blackList.Add(exportType.FullName);
        }

        /// <summary>
        /// Name of scope
        /// </summary>
        public string ScopeName
        {
            get { return RootScope.ScopeName; }
        }

        /// <summary>
        /// Unique Scope Id
        /// </summary>
        public Guid ScopeId
        {
            get { return RootScope.ScopeId; }
        }

        /// <summary>
        /// Adds a secondary resolver to the container.
        /// </summary>
        /// <param name="newLocator">new secondary locator</param>
        public void AddSecondaryLocator(ISecondaryExportLocator newLocator)
        {
            RootScope.AddSecondaryLocator(newLocator);
        }

        /// <summary>
        /// List of Export Locators
        /// </summary>
        public IEnumerable<ISecondaryExportLocator> SecondaryExportLocators
        {
            get { return RootScope.SecondaryExportLocators; }
        }

        /// <summary>
        /// Add a strategy 
        /// </summary>
        /// <param name="inspector">strategy inspector</param>
        public void AddStrategyInspector(IStrategyInspector inspector)
        {
            RootScope.AddStrategyInspector(inspector);
        }

        /// <summary>
        /// This method can be used to configure the root scope of the container
        /// </summary>
        /// <param name="registrationDelegate">configuration delegate</param>
        public void Configure(ExportRegistrationDelegate registrationDelegate)
        {
            RootScope.Configure(registrationDelegate);
        }

        /// <summary>
        /// This method can be used to configure a particular scope in the container
        /// </summary>
        /// <param name="configurationModule"></param>
        /// <param name="scopeName"></param>
        public void Configure(string scopeName, IConfigurationModule configurationModule)
        {
            _injectionKernelManager.Configure(scopeName, configurationModule.Configure);
        }

        /// <summary>
        /// Missing export strategy providers can provide a set of exports that can be used to resolve a satisfy an import
        /// </summary>
        /// <param name="exportStrategyProvider">export strategy provider</param>
        public void AddMissingExportStrategyProvider(IMissingExportStrategyProvider exportStrategyProvider)
        {
            RootScope.AddMissingExportStrategyProvider(exportStrategyProvider);
        }

        /// <summary>
        /// Add IStrategyInspector to the container. It will be called 
        /// </summary>
        /// <param name="inspector"></param>
        public void AddInspector(IStrategyInspector inspector)
        {
            RootScope.AddInspector(inspector);
        }

        /// <summary>
        /// Add an object for disposal 
        /// </summary>
        /// <param name="disposable"></param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        public void AddDisposable(IDisposable disposable, BeforeDisposalCleanupDelegate cleanupDelegate = null)
        {
            RootScope.AddDisposable(disposable, cleanupDelegate);
        }

        /// <summary>
        /// Remove an object from the disposal scope
        /// </summary>
        /// <param name="disposable"></param>
        public void RemoveDisposable(IDisposable disposable)
        {
            RootScope.RemoveDisposable(disposable);
        }

        /// <summary>
        /// Creates a child scope from this scope
        /// </summary>
        /// <param name="scopeName">name of the scope you want to create</param>
        /// <param name="registrationDelegate"></param>
        /// <param name="disposalScopeProvider"></param>
        /// <returns></returns>
        public IInjectionScope CreateChildScope(ExportRegistrationDelegate registrationDelegate = null,
            string scopeName = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            return RootScope.CreateChildScope(registrationDelegate, scopeName, disposalScopeProvider);
        }

        /// <summary>
        /// Creates a child scope from this scope using a configuration module
        /// </summary>
        /// <param name="scopeName">name of the scope you want to create</param>
        /// <param name="configurationModule"></param>
        /// <param name="disposalScopeProvider"></param>
        /// <returns></returns>
        public IInjectionScope CreateChildScope(IConfigurationModule configurationModule,
            string scopeName = null,
            IDisposalScopeProvider disposalScopeProvider = null)
        {
            return RootScope.CreateChildScope(configurationModule, scopeName, disposalScopeProvider);
        }

        /// <summary>
        /// Create an injection context
        /// </summary>
        /// <returns></returns>
        public IInjectionContext CreateContext(IDisposalScope disposalScope = null)
        {
            return RootScope.CreateContext(disposalScope);
        }

        /// <summary>
        /// This method can be used to configure the root scope of the container
        /// </summary>
        /// <param name="configurationModule"></param>
        public void Configure(IConfigurationModule configurationModule)
        {
            RootScope.Configure(configurationModule);
        }

        /// <summary>
        /// This method can be used to configure a particular scope in the container
        /// </summary>
        /// <param name="scopeName">the name of the scope that is being configured</param>
        /// <param name="registrationDelegate">configuration delegate</param>
        public void Configure(string scopeName, ExportRegistrationDelegate registrationDelegate)
        {
            _injectionKernelManager.Configure(scopeName, registrationDelegate);
        }

        /// <summary>
        /// By handling this event you can provide a value when no export was found
        /// </summary>
        public event EventHandler<ResolveUnknownExportArgs> ResolveUnknownExports;

        /// <summary>
        /// Locate an export by type
        /// </summary>
        /// <param name="injectionContext">injection context for the locate</param>
        /// <param name="consider">filter to be used when locating</param>
        /// <param name="withKey"></param>
        /// <typeparam name="T">type to locate</typeparam>
        /// <returns>export T if found, other wise default(T)</returns>
        public T Locate<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            return RootScope.Locate<T>(injectionContext, consider, withKey);
        }

        /// <summary>
        /// Locate an object by type
        /// </summary>
        /// <param name="objectType">type to locate</param>
        /// <param name="injectionContext">injection context to use while locating</param>
        /// <param name="consider">filter to use while locating export</param>
        /// <param name="withKey"></param>
        /// <returns>export object if found, other wise null</returns>
        public object Locate(Type objectType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            return RootScope.Locate(objectType, injectionContext, consider, withKey);
        }

        /// <summary>
        /// Locate an export by name
        /// </summary>
        /// <param name="exportName">name of export to locate</param>
        /// <param name="injectionContext">injection context to use while locating</param>
        /// <param name="consider">filter to use while locating</param>
        /// <param name="withKey"></param>
        /// <returns>export object if found, other wise null</returns>
        public object Locate(string exportName, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            return RootScope.Locate(exportName, injectionContext, consider, withKey);
        }

        /// <summary>
        /// Locate all export of type T
        /// </summary>
        /// <param name="injectionContext">injection context to use while locating</param>
        /// <param name="consider">filter to use while locating</param>
        /// <param name="withKey"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T">type to locate</typeparam>
        /// <returns>List of T, this will return an empty list if not exports are found</returns>
        public List<T> LocateAll<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null, IComparer<T> comparer = null)
        {
            return RootScope.LocateAll(injectionContext, consider, withKey, comparer);
        }

        /// <summary>
        /// Locate All exports by the name provided
        /// </summary>
        /// <param name="name">export name to locate</param>
        /// <param name="injectionContext">injection context to use while locating</param>
        /// <param name="consider">filter to use while locating</param>
        /// <param name="withKey"></param>
        /// <param name="comparer"></param>
        /// <returns>List of objects, this will return an empty list if no exports are found</returns>
        public List<object> LocateAll(string name, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null, IComparer<object> comparer = null)
        {
            return RootScope.LocateAll(name, injectionContext, consider, withKey, comparer);
        }

        /// <summary>
        /// Locate all exports by type
        /// </summary>
        /// <param name="exportType">type to locate</param>
        /// <param name="injectionContext">injection context</param>
        /// <param name="consider">filter to use while locating</param>
        /// <param name="withKey"></param>
        /// <param name="comparer"></param>
        /// <returns>list of object, this will return an empty list if no exports are found</returns>
        public List<object> LocateAll(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null, IComparer<object> comparer = null)
        {
            return RootScope.LocateAll(exportType, injectionContext, consider, withKey, comparer);
        }

        /// <summary>
        /// The environment for this scope (always inherited from the root scope)
        /// </summary>
        public ExportEnvironment Environment
        {
            get { return RootScope.Environment; }
        }

        /// <summary>
        /// Returns a list of all known strategies.
        /// </summary>
        /// <param name="exportFilter"></param>
        /// <returns>returns all known strategies</returns>
        public IEnumerable<IExportStrategy> GetAllStrategies(ExportStrategyFilter exportFilter = null)
        {
            return RootScope.GetAllStrategies(exportFilter);
        }

        /// <summary>
        /// Finds the best matching strategy exported by the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <param name="injectionContext"></param>
        /// <param name="exportFilter"></param>
        /// <param name="withKey"></param>
        /// <returns></returns>
        public IExportStrategy GetStrategy(string name, IInjectionContext injectionContext = null, ExportStrategyFilter exportFilter = null, object withKey = null)
        {
            return RootScope.GetStrategy(name, injectionContext, exportFilter, withKey);
        }

        /// <summary>
        /// Finds the best matching strategy exported by the name provided
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="injectionContext"></param>
        /// <param name="exportFilter"></param>
        /// <param name="withKey"></param>
        /// <returns></returns>
        public IExportStrategy GetStrategy(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter exportFilter = null, object withKey = null)
        {
            return RootScope.GetStrategy(exportType, injectionContext, exportFilter, withKey);
        }

        /// <summary>
        /// Get the list of exported strategies sorted by best option.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="injectionContext"></param>
        /// <param name="exportFilter"></param>
        /// <returns></returns>
        public IEnumerable<IExportStrategy> GetStrategies(string name,
            IInjectionContext injectionContext = null,
            ExportStrategyFilter exportFilter = null)
        {
            return RootScope.GetStrategies(name, injectionContext, exportFilter);
        }

        /// <summary>
        /// Get the list of exported strategies sorted by best option.
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="injectionContext"></param>
        /// <param name="exportFilter"></param>
        /// <returns></returns>
        public IEnumerable<IExportStrategy> GetStrategies(Type exportType,
            IInjectionContext injectionContext = null,
            ExportStrategyFilter exportFilter = null)
        {
            return RootScope.GetStrategies(exportType, injectionContext, exportFilter);
        }

        /// <summary>
        /// Get the export strategy collection
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns>can be null if nothing is registered by that name</returns>
        public IExportStrategyCollection GetStrategyCollection(Type exportType)
        {
            return RootScope.GetStrategyCollection(exportType);
        }

        public IExportStrategyCollection GetStrategyCollection(string exportName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new strategy to the container
        /// </summary>
        /// <param name="addStrategy"></param>
        public void AddStrategy(IExportStrategy addStrategy)
        {
            RootScope.AddStrategy(addStrategy);
        }

        /// <summary>
        /// Allows the caller to remove a strategy from the container
        /// </summary>
        /// <param name="knownStrategy">strategy to remove</param>
        public void RemoveStrategy(IExportStrategy knownStrategy)
        {
            RootScope.RemoveStrategy(knownStrategy);
        }

        /// <summary>
        /// Inject dependencies into a constructed object
        /// </summary>
        /// <param name="injectedObject">object to be injected</param>
        /// <param name="injectionContext">injection context</param>
        public void Inject(object injectedObject, IInjectionContext injectionContext = null)
        {
            RootScope.Inject(injectedObject, injectionContext);
        }

        /// <summary>
        /// List of Injection Inspectors for the scope
        /// </summary>
        public IEnumerable<IStrategyInspector> Inspectors
        {
            get { return RootScope.Inspectors; }
        }

        /// <summary>
        /// Dispose of the container
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Locate missing exports
        /// </summary>
        /// <param name="context">injection context</param>
        /// <param name="exportName">export name</param>
        /// <param name="exportType">export type</param>
        /// <param name="consider">export filter</param>
        /// <param name="locateKey"></param>
        /// <returns>export object</returns>
        public object LocateMissingExport(IInjectionContext context, string exportName, Type exportType, ExportStrategyFilter consider, object locateKey)
        {
            object returnValue = null;

            foreach (IMissingExportStrategyProvider missingExportStrategyProvider in _missingExportStrategyProviders)
            {
                var exports = missingExportStrategyProvider.ProvideExports(context,
                    exportName,
                    exportType,
                    consider,
                    locateKey);

                foreach (IExportStrategy exportStrategy in exports)
                {
                    RootScope.AddStrategy(exportStrategy);

                }

                IExportStrategy strategy = exportType != null ? 
                                           RootScope.GetStrategy(exportType, context, consider, locateKey) : 
                                           RootScope.GetStrategy(exportName, context, consider, locateKey);

                if (strategy != null)
                {
                    returnValue = strategy.Activate(RootScope, context, consider, locateKey);
                }
            }

            if (returnValue == null)
            {
                EventHandler<ResolveUnknownExportArgs> missingExportEvent = ResolveUnknownExports;

                if (missingExportEvent != null)
                {
                    ResolveUnknownExportArgs eventArgs = new ResolveUnknownExportArgs(context,
                        exportName,
                        exportType,
                        locateKey);

                    missingExportEvent(this, eventArgs);

                    return eventArgs.ExportedValue;
                }
            }

            return null;
        }

        /// <summary>
        /// This method compares 2 export strategies by class name
        /// </summary>
        /// <param name="x">x compare object</param>
        /// <param name="y">y compare object</param>
        /// <param name="environment">environment to compare the strategies in</param>
        /// <returns>compare value</returns>
        public static int CompareExportStrategiesByName(IExportStrategy x, IExportStrategy y, ExportEnvironment environment)
        {
            return string.Compare(x.ActivationType.Name, y.ActivationType.Name, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// This method compares 2 export strategies in a particular environment using ExportEnvironment attributes and ExportPriority attributes
        /// </summary>
        /// <param name="x">x compare object</param>
        /// <param name="y">y compare object</param>
        /// <param name="environment">environment to compare the strategies in</param>
        /// <returns>compare value</returns>
        public static int CompareExportStrategies(IExportStrategy x, IExportStrategy y, ExportEnvironment environment)
        {
            int returnValue = 0;

            if (x.Environment != y.Environment)
            {
                switch (environment)
                {
                    case ExportEnvironment.RunTime:
                        returnValue = CompareValues(x, y, ExportEnvironment.RunTime, ExportEnvironment.RunTimeOnly);
                        break;

                    case ExportEnvironment.UnitTest:
                        returnValue = CompareValues(x, y, ExportEnvironment.UnitTest, ExportEnvironment.UnitTestOnly);
                        break;

                    case ExportEnvironment.DesignTime:
                        returnValue = CompareValues(x, y, ExportEnvironment.DesignTime, ExportEnvironment.DesignTimeOnly);
                        break;
                }
            }

            if (returnValue == 0)
            {
                if (x.Priority > y.Priority)
                {
                    returnValue = 1;
                }
                else if (x.Priority < y.Priority)
                {
                    returnValue = -1;
                }
                else if (x.ActivationType != null &&
                            y.ActivationType != null)
                {
                    // all things being equal sort alphabetically by class name
                    returnValue = string.Compare(x.ActivationType.Name, y.ActivationType.Name, StringComparison.CurrentCulture);
                }
            }

            return returnValue;
        }

        private static int CompareValues(IExportStrategy x,
            IExportStrategy y,
            ExportEnvironment exportEnvironment,
            ExportEnvironment exportEnvironmentOnly)
        {
            int returnValue = 0;

            if (x.Environment == exportEnvironment || x.Environment == exportEnvironmentOnly)
            {
                returnValue++;
            }

            if (y.Environment == exportEnvironment || y.Environment == exportEnvironmentOnly)
            {
                returnValue--;
            }

            if (returnValue == 0)
            {
                if (x.Environment == ExportEnvironment.Any)
                {
                    returnValue++;
                }

                if (y.Environment == ExportEnvironment.Any)
                {
                    returnValue--;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// dispose implementation
        /// </summary>
        /// <param name="dispose"></param>
        protected virtual void Dispose(bool dispose)
        {
            if (_disposed)
            {
                return;
            }

            if (RootScope != null)
            {
                RootScope.Dispose();
            }
        }

        // ReSharper disable once UnusedMember.Local
        private string DebugDisplayString
        {
            get { return "Exports: " + GetAllStrategies().Count(); }
        }
    }
}