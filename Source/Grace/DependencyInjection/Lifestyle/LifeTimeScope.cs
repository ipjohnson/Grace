using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;
using JetBrains.Annotations;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// The intention of the lifetime scope is to act as a light weight container.
	/// It does not support adding new exports or locators too
	/// </summary>
	public sealed class LifetimeScope : IInjectionScope
	{
        private readonly IKernelConfiguration _configuration;
		private readonly IDisposalScope _disposalScope;
		private readonly object _extraDataLock = new object();
		private volatile Dictionary<string, object> _extraData;

		/// <summary>
		/// Default lifetime scope
		/// </summary>
		/// <param name="parentLocator"></param>
        /// <param name="configuration"></param>
		/// <param name="scopeName"></param>
		public LifetimeScope([NotNull] IInjectionScope parentLocator, IKernelConfiguration configuration, string scopeName = null)
		{
			if (parentLocator == null)
			{
				throw new ArgumentNullException("parentLocator");
			}

            if(configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _configuration = configuration;
			_disposalScope = new DisposalScope();
			ScopeName = scopeName ?? string.Empty;
			ScopeId = Guid.NewGuid();
			ParentScope = parentLocator;
		}

		/// <summary>
		/// Unique identifier for the instance of the injection scope
		/// </summary>
		public Guid ScopeId { get; private set; }

		/// <summary>
		/// The scopes name
		/// </summary>
		public string ScopeName { get; private set; }

        /// <summary>
        /// Configuration
        /// </summary>
        public IKernelConfiguration Configuration {  get { return _configuration; } }

		/// <summary>
		/// List of Export Locators
		/// </summary>
		public IEnumerable<ISecondaryExportLocator> SecondaryExportLocators
		{
			get { return ImmutableArray<ISecondaryExportLocator>.Empty; }
		}

		/// <summary>
		/// Creates a child scope from this scope
		/// </summary>
		/// <param name="scopeName">name of the scope you want to create</param>
		/// <param name="registrationDelegate">delegate used to configure the new child scope</param>
		/// <param name="disposalScopeProvider">new disposal scope provider for the child scope</param>
		/// <returns>new child scope</returns>
		public IInjectionScope CreateChildScope(ExportRegistrationDelegate registrationDelegate = null,
			string scopeName = null,
			IDisposalScopeProvider disposalScopeProvider = null)
		{
			if (registrationDelegate != null)
			{
				throw new ArgumentException("registration delegate must be null");
			}

			return new LifetimeScope(this, _configuration, scopeName);
		}

		/// <summary>
		/// Creates a child scope from this scope using a configuration module
		/// </summary>
		/// <param name="scopeName">name of the scope you want to create</param>
		/// <param name="configurationModule">configuration module used to configure the new child scope</param>
		/// <param name="disposalScopeProvider">new disposal scope for the child scope</param>
		/// <returns>new child scope</returns>
		public IInjectionScope CreateChildScope(IConfigurationModule configurationModule,
			string scopeName = null,
			IDisposalScopeProvider disposalScopeProvider = null)
		{
			if (configurationModule != null)
			{
				throw new ArgumentException("configuration Module must be null");
			}

			return new LifetimeScope(this, _configuration, scopeName);
		}

		/// <summary>
		/// Create an injection context
		/// </summary>
		/// <returns>new injection context</returns>
		public IInjectionContext CreateContext(IDisposalScope disposalScope = null)
        {
            if (disposalScope == null)
            {
                disposalScope = _configuration.DisposalScopeProvider == null ?
                                    this :
                                    _configuration.DisposalScopeProvider.ProvideDisposalScope(this);
            }

            return _configuration.ContextCreation(disposalScope, this);
        }

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
            if (injectionContext == null)
            {
                injectionContext = CreateContext();
            }

            return ParentScope.Locate<T>(injectionContext, consider, withKey);
		}

		/// <summary>
		/// Locate an object by type
		/// </summary>
		/// <param name="objectType">type to locate</param>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating export</param>
		/// <param name="withKey"></param>
		/// <returns>export object if found, other wise null</returns>
		public object Locate(Type objectType,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null,
			object withKey = null)
		{
            if (injectionContext == null)
            {
                injectionContext = CreateContext();
            }

            return ParentScope.Locate(objectType, injectionContext, consider, withKey);
		}

		/// <summary>
		/// Locate an export by name
		/// </summary>
		/// <param name="exportName">name of export to locate</param>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <returns>export object if found, other wise null</returns>
		public object Locate(string exportName,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null,
			object withKey = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.Locate(exportName, injectionContext, consider, withKey);
		}

		/// <summary>
		/// Locate all export of type T
		/// </summary>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <param name="comparer">used for sorting the imports when returning the list</param>
		/// <typeparam name="T">type to locate</typeparam>
		/// <returns>List of T, this will return an empty list if not exports are found</returns>
		public List<T> LocateAll<T>(IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null,
			object withKey = null,
			IComparer<T> comparer = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.LocateAll<T>(injectionContext, consider, withKey, comparer);
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
		public List<object> LocateAll(string name,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null,
			object withKey = null,
			IComparer<object> comparer = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.LocateAll(name, injectionContext, consider, withKey, comparer);
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
		public List<object> LocateAll(Type exportType,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter consider = null,
			object withKey = null,
			IComparer<object> comparer = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.LocateAll(exportType, injectionContext, consider, withKey, comparer);
		}
        
		public IEnumerable<IExportStrategy> GetAllStrategies(ExportStrategyFilter exportFilter = null)
		{
			return ParentScope.GetAllStrategies(exportFilter);
		}

		/// <summary>
		/// Finds the best matching strategy exported by the name provided
		/// </summary>
		/// <param name="name"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <param name="withKey"></param>
		/// <returns></returns>
		public IExportStrategy GetStrategy(string name,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter exportFilter = null,
			object withKey = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.GetStrategy(name, injectionContext, exportFilter, withKey);
		}

		/// <summary>
		/// Finds the best matching strategy exported by the name provided
		/// </summary>
		/// <param name="exportType">type to locate</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportFilter">export filter</param>
		/// <param name="withKey"></param>
		/// <returns></returns>
		public IExportStrategy GetStrategy(Type exportType,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter exportFilter = null,
			object withKey = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.GetStrategy(exportType, injectionContext, exportFilter, withKey);
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
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.GetStrategies(name, injectionContext, exportFilter);
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
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			return ParentScope.GetStrategies(exportType, injectionContext, exportFilter);
		}

	    /// <summary>
	    /// Get the export strategy collection
	    /// </summary>
	    /// <param name="exportType"></param>
	    /// <param name="createIfDoesntExist"></param>
	    /// <returns>can be null if nothing is registered by that name</returns>
	    public IExportStrategyCollection GetStrategyCollection(Type exportType, bool createIfDoesntExist = true)
		{
			return null;
		}

	    /// <summary>
	    /// Get the export collection by name
	    /// </summary>
	    /// <param name="exportName">export name</param>
	    /// <param name="createIfDoesntExist"></param>
	    /// <returns></returns>
	    public IExportStrategyCollection GetStrategyCollection(string exportName, bool createIfDoesntExist = true)
	    {
	        return null;
	    }

	    #region Not implemented

		/// <summary>
		/// Adds a secondary resolver to the container.
		/// </summary>
		/// <param name="newLocator">new secondary locator</param>
		public void AddSecondaryLocator(ISecondaryExportLocator newLocator)
		{
			throw new NotImplementedException("This feature is not supported");
		}

		/// <summary>
		/// Add a strategy 
		/// </summary>
		/// <param name="inspector">strategy inspector</param>
        public void AddStrategyInspector(IExportStrategyInspector inspector)
		{
			throw new NotImplementedException("This feature is not supported");
		}

		/// <summary>
		/// This method can be used to configure the root scope of the container
		/// </summary>
		/// <param name="registrationDelegate">registration delegate used to configure the locator</param>
		public void Configure(ExportRegistrationDelegate registrationDelegate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method can be used to configure the root scope of the container
		/// </summary>
		/// <param name="configurationModule">configuration module used to configure the locator</param>
		public void Configure(IConfigurationModule configurationModule)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds a new strategy to the container
		/// </summary>
		/// <param name="addStrategy"></param>
		public void AddStrategy(IExportStrategy addStrategy)
		{
			throw new NotImplementedException("Not supported");
		}

		/// <summary>
		/// Allows the caller to remove a strategy from the container
		/// </summary>
		/// <param name="knownStrategy">strategy to remove</param>
		public void RemoveStrategy(IExportStrategy knownStrategy)
		{
			throw new NotImplementedException("Not supported");
		}

		/// <summary>
		/// adds disposable
		/// </summary>
		/// <param name="disposable"></param>
		/// <param name="cleanupDelegate"></param>
		public void AddDisposable(IDisposable disposable, BeforeDisposalCleanupDelegate cleanupDelegate = null)
		{
			_disposalScope.AddDisposable(disposable, cleanupDelegate);
		}

		/// <summary>
		/// Remove disposalble
		/// </summary>
		/// <param name="disposable"></param>
		public void RemoveDisposable(IDisposable disposable)
		{
			_disposalScope.RemoveDisposable(disposable);
		}
		#endregion

		/// <summary>
		/// Inject dependencies into a constructed object
		/// </summary>
		/// <param name="injectedObject">object to be injected</param>
		/// <param name="injectionContext">injection context</param>
		public void Inject(object injectedObject, IInjectionContext injectionContext = null)
		{
			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			ParentScope.Inject(injectedObject, injectionContext);
		}


		#region ExtraData Methods

		/// <summary>
		/// Extra data associated with the injection request. 
		/// </summary>
		/// <param name="dataName"></param>
		/// <returns></returns>
		public object GetExtraData(string dataName)
		{
			object returnValue = null;

			if (_extraData != null)
			{
				lock (_extraDataLock)
				{
					_extraData.TryGetValue(dataName, out returnValue);
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Sets extra data on the injection context
		/// </summary>
		/// <param name="dataName"></param>
		/// <param name="newValue"></param>
		public void SetExtraData(string dataName, object newValue)
		{
			lock (_extraDataLock)
			{
				if (_extraData == null)
				{
					_extraData = new Dictionary<string, object>();
				}

				_extraData[dataName] = newValue;
			}
		}

		#endregion

		/// <summary>
		/// Dispose of lifetime
		/// </summary>
		public void Dispose()
		{
			_disposalScope.Dispose();
		}

		/// <summary>
		/// Container
		/// </summary>
		public IDependencyInjectionContainer Container
		{
			get { return ParentScope.Container; }
		}

		/// <summary>
		/// Parent scope
		/// </summary>
		public IInjectionScope ParentScope { get; private set; }

	    /// <summary>
	    /// List of missing exports providers
	    /// </summary>
	    public IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders
	    {
	        get { yield break; }
	    }

	    public IEnumerable<IExportStrategyInspector> Inspectors
	    {
            get { return ImmutableArray<IExportStrategyInspector>.Empty; }
	    }

        public IEnumerable<IInjectionValueProviderInspector> InjectionInspectors
        {
            get
            {
                return ImmutableArray<IInjectionValueProviderInspector>.Empty;
            }
        }

        public void AddMissingExportStrategyProvider(IMissingExportStrategyProvider exportStrategyProvider)
	    {
            throw new NotSupportedException();
	    }

        public bool TryLocate<T>(out T value, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            if (injectionContext == null)
            {
                injectionContext = CreateContext();
            }

            return ParentScope.TryLocate(out value, injectionContext, consider, withKey);
        }

        public bool TryLocate(Type type, out object value, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null)
        {
            if (injectionContext == null)
            {
                injectionContext = CreateContext();
            }

            return ParentScope.TryLocate(type, out value, injectionContext, consider, withKey);
        }

        public void AddInjectionValueProviderInspector([NotNull] IInjectionValueProviderInspector inspector)
        {
            throw new NotSupportedException("Not supported by lifetime scope");
        }
    }
}
