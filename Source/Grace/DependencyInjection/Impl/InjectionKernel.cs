using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl.DelegateFactory;
using Grace.Diagnostics;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// InjectionKernel keeps a collection of exports to be used for resolving dependencies.
	/// </summary>
	[DebuggerDisplay("{DebugDisplayString,nq}")]
	[DebuggerTypeProxy(typeof(InjectionScopeDiagnostic))]
	public class InjectionKernel : DisposalScope, IInjectionScope, IMissingExportHandler
	{
		#region Private Static

		private static readonly ReadOnlyDictionary<string, ExportStrategyCollection> emptyStringExportStrategies =
			new ReadOnlyDictionary<string, ExportStrategyCollection>(new Dictionary<string, ExportStrategyCollection>());

		private static readonly ReadOnlyDictionary<Type, ExportStrategyCollection> emptyTypeExportStrategies =
			new ReadOnlyDictionary<Type, ExportStrategyCollection>(new Dictionary<Type, ExportStrategyCollection>());

		private static readonly ReadOnlyDictionary<Type, IInjectionStrategy> emptyInjectionStrategies =
			new ReadOnlyDictionary<Type, IInjectionStrategy>(new Dictionary<Type, IInjectionStrategy>());

		private static readonly MethodInfo locateLazyListMethodInfo;
		private static readonly MethodInfo locateOwnedListMethodInfo;
		private static readonly MethodInfo locateMetaListMethodInfo;
		private static readonly Dictionary<Type, Type> openGenericStrategyMapping;

		static InjectionKernel()
		{
			locateLazyListMethodInfo = typeof(InjectionKernel).GetRuntimeMethods()
				.First(x => x.Name == "LocateListOfLazyExports");
			locateMetaListMethodInfo = typeof(InjectionKernel).GetRuntimeMethods()
				.First(x => x.Name == "LocateListOfMetaExports");
			locateOwnedListMethodInfo = typeof(InjectionKernel).GetRuntimeMethods()
				.First(x => x.Name == "LocateListOfOwnedExports");

			openGenericStrategyMapping = new Dictionary<Type, Type>();

			openGenericStrategyMapping[typeof(IEnumerable<>)] = typeof(ListExportStrategy<>);
			openGenericStrategyMapping[typeof(ICollection<>)] = typeof(ListExportStrategy<>);
			openGenericStrategyMapping[typeof(IList<>)] = typeof(ListExportStrategy<>);
			openGenericStrategyMapping[typeof(List<>)] = typeof(ListExportStrategy<>);

			openGenericStrategyMapping[typeof(IReadOnlyCollection<>)] = typeof(ReadOnlyCollectionExportStrategy<>);
			openGenericStrategyMapping[typeof(IReadOnlyList<>)] = typeof(ReadOnlyCollectionExportStrategy<>);
			openGenericStrategyMapping[typeof(ReadOnlyCollection<>)] = typeof(ReadOnlyCollectionExportStrategy<>);

			openGenericStrategyMapping[typeof(Owned<>)] = typeof(OwnedStrategy<>);
			openGenericStrategyMapping[typeof(Meta<>)] = typeof(MetaStrategy<>);
			openGenericStrategyMapping[typeof(Lazy<>)] = typeof(LazyExportStrategy<>);

			openGenericStrategyMapping[typeof(Func<>)] = typeof(FuncExportStrategy<>);

			openGenericStrategyMapping[typeof(Func<,>)] = typeof(GenericFuncExportStrategy<,>);
			openGenericStrategyMapping[typeof(Func<,,>)] = typeof(GenericFuncExportStrategy<,,>);
			openGenericStrategyMapping[typeof(Func<,,,>)] = typeof(GenericFuncExportStrategy<,,,>);
			openGenericStrategyMapping[typeof(Func<,,,,>)] = typeof(GenericFuncExportStrategy<,,,,>);
			openGenericStrategyMapping[typeof(Func<,,,,,>)] = typeof(GenericFuncExportStrategy<,,,,,>);
		}

		#endregion

		#region Private Fields

		private readonly ExportStrategyComparer comparer;
		private readonly IDisposalScopeProvider disposalScopeProvider;
		private readonly object exportsLock = new object();
		private readonly object extraDataLock = new object();
		private readonly InjectionKernelManager kernelManager;
		private readonly ILog log = Logger.GetLogger<InjectionKernel>();
		private readonly object secondaryResolversLock = new object();
		private volatile Dictionary<string, object> extraData;
		private volatile ReadOnlyDictionary<string, ExportStrategyCollection> exportsByName;
		private volatile ReadOnlyDictionary<Type, ExportStrategyCollection> exportsByType;
		private volatile ReadOnlyDictionary<Type, IInjectionStrategy> injections;
		private volatile ReadOnlyCollection<ISecondaryExportLocator> secondaryResolvers;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="kernelManager">kernel manager for this kernel</param>
		/// <param name="parentScope"></param>
		/// <param name="scopeProvider">passing a null for scope provider is ok</param>
		/// <param name="scopeName"></param>
		/// <param name="comparer"></param>	
		public InjectionKernel(InjectionKernelManager kernelManager,
			IInjectionScope parentScope,
			IDisposalScopeProvider scopeProvider,
			string scopeName,
			ExportStrategyComparer comparer)
		{
			ScopeId = Guid.NewGuid();

			exportsByName = emptyStringExportStrategies;
			exportsByType = emptyTypeExportStrategies;
			injections = emptyInjectionStrategies;

			this.kernelManager = kernelManager;
			this.comparer = comparer;
			disposalScopeProvider = scopeProvider;
			ScopeName = scopeName;
			ParentScope = parentScope;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The container this scope was created in
		/// </summary>
		public IDependencyInjectionContainer Container
		{
			get { return kernelManager.Container; }
		}

		/// <summary>
		/// Unique identifier for the instance of the injection scope
		/// </summary>
		public Guid ScopeId { get; private set; }

		/// <summary>
		/// The scopes name
		/// </summary>
		public string ScopeName { get; internal set; }

		/// <summary>
		/// Parent scope, can be null if it's the root scope
		/// </summary>
		public IInjectionScope ParentScope { get; internal set; }

		/// <summary>
		/// The environment for this scope (always inherited from the root scope)
		/// </summary>
		public ExportEnvironment Environment { get; internal set; }

		#endregion

		#region Scope Methods

		/// <summary>
		/// Creates a child scope from this scope
		/// </summary>
		/// <param name="scopeName"></param>
		/// <param name="registrationDelegate"></param>
		/// <param name="disposalScopeProvider"></param>
		/// <returns></returns>
		public IInjectionScope CreateChildScope(ExportRegistrationDelegate registrationDelegate = null,
			string scopeName = null,
			IDisposalScopeProvider disposalScopeProvider = null)
		{
			IInjectionScope returnValue =
				kernelManager.CreateNewKernel(this,
					scopeName,
					registrationDelegate,
					this.disposalScopeProvider,
					disposalScopeProvider);

			return returnValue;
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
			return CreateChildScope(configurationModule.Configure, scopeName, disposalScopeProvider);
		}

		public string Name { get; private set; }

		/// <summary>
		/// Adds a secondary resolver to the injection scope
		/// </summary>
		/// <param name="newLocator"></param>
		public void AddSecondaryLocator(ISecondaryExportLocator newLocator)
		{
			lock (secondaryResolversLock)
			{
				List<ISecondaryExportLocator> newResolvers = new List<ISecondaryExportLocator>(1);

				if (secondaryResolvers != null)
				{
					newResolvers.AddRange(secondaryResolvers);
				}

				newResolvers.Add(newLocator);

				secondaryResolvers = new ReadOnlyCollection<ISecondaryExportLocator>(newResolvers);
			}
		}

		/// <summary>
		/// List of Export Locators
		/// </summary>
		public IEnumerable<ISecondaryExportLocator> SecondaryExportLocators
		{
			get
			{
				if (secondaryResolvers != null)
				{
					return secondaryResolvers;
				}

				return new ISecondaryExportLocator[0];
			}
		}

		/// <summary>
		/// Add a strategy 
		/// </summary>
		/// <param name="inspector">strategy inspector</param>
		public void AddStrategyInspector(IStrategyInspector inspector)
		{
		}

		/// <summary>
		/// Clone the injection kernel, the rootscope cannot be cloned
		/// </summary>
		/// <param name="parentScope"></param>
		/// <param name="parentScopeProvider"></param>
		/// <param name="scopeProvider"></param>
		/// <returns></returns>
		public InjectionKernel Clone(IInjectionScope parentScope,
			IDisposalScopeProvider parentScopeProvider,
			IDisposalScopeProvider scopeProvider)
		{
			if (ParentScope == null)
			{
				throw new RootScopeCloneException(ScopeName, ScopeId);
			}

			IDisposalScopeProvider newProvider = (scopeProvider ?? disposalScopeProvider) ?? parentScopeProvider;

			InjectionKernel returnValue = new InjectionKernel(kernelManager, parentScope, newProvider, ScopeName, comparer)
													{
														ParentScope = parentScope,
														Environment = Environment,
														exportsByName = exportsByName,
														exportsByType = exportsByType
													};

			return returnValue;
		}

		#endregion

		#region Configure Methods

		/// <summary>
		/// You can add extra configuration to the scope
		/// </summary>
		/// <param name="registrationDelegate"></param>
		public void Configure(ExportRegistrationDelegate registrationDelegate)
		{
			ExportRegistrationBlock registrationBlock = new ExportRegistrationBlock(this);

			registrationDelegate(registrationBlock);

			List<IExportStrategy> exportStrategyList = new List<IExportStrategy>();
			IExportStrategy[] exportStrategies = registrationBlock.GetExportStrategies().ToArray();

			foreach (IExportStrategy exportStrategy in exportStrategies)
			{
				if (kernelManager.BlackList.IsExportStrategyBlackedOut(exportStrategy))
				{
					continue;
				}

				exportStrategy.OwningScope = this;

				exportStrategy.Initialize();

				exportStrategyList.Add(exportStrategy);

				foreach (IExportStrategy secondaryStrategy in exportStrategy.SecondaryStrategies())
				{
					secondaryStrategy.OwningScope = this;

					secondaryStrategy.Initialize();

					exportStrategyList.Add(secondaryStrategy);
				}
			}

			AddExportStrategies(exportStrategyList);
		}

		private void AddExportStrategies(List<IExportStrategy> exportStrategyList)
		{
			lock (exportsLock)
			{
				Dictionary<string, ExportStrategyCollection> newExportsByName =
					new Dictionary<string, ExportStrategyCollection>(exportsByName);

				foreach (IExportStrategy exportStrategy in exportStrategyList)
				{
					if (kernelManager.BlackList.IsExportStrategyBlackedOut(exportStrategy))
					{
						continue;
					}

					IEnumerable<string> exportNames = exportStrategy.ExportNames;

					if (exportNames != null)
					{
						foreach (string exportName in exportStrategy.ExportNames)
						{
							string lowerName = exportName.ToLowerInvariant();
							ExportStrategyCollection currentCollection;

							if (!newExportsByName.TryGetValue(lowerName, out currentCollection))
							{
								currentCollection = new ExportStrategyCollection(this, Environment, comparer);

								newExportsByName[lowerName] = currentCollection;
							}

							currentCollection.AddExport(exportStrategy);
						}
					}
				}

				Dictionary<Type, ExportStrategyCollection> newExportsByType =
					new Dictionary<Type, ExportStrategyCollection>(exportsByType);

				foreach (IExportStrategy exportStrategy in exportStrategyList)
				{
					if (kernelManager.BlackList.IsExportStrategyBlackedOut(exportStrategy))
					{
						continue;
					}

					IEnumerable<Type> exportTypes = exportStrategy.ExportTypes;

					if (exportTypes != null)
					{
						foreach (Type exportType in exportStrategy.ExportTypes)
						{
							ExportStrategyCollection currentCollection;

							if (!newExportsByType.TryGetValue(exportType, out currentCollection))
							{
								currentCollection = new ExportStrategyCollection(this, Environment, comparer);

								newExportsByType[exportType] = currentCollection;
							}

							currentCollection.AddExport(exportStrategy);
						}
					}
				}

				exportsByType = new ReadOnlyDictionary<Type, ExportStrategyCollection>(newExportsByType);
				exportsByName = new ReadOnlyDictionary<string, ExportStrategyCollection>(newExportsByName);
			}
		}

		/// <summary>
		/// Configure the scope with a configuration module
		/// </summary>
		/// <param name="configurationModule"></param>
		public void Configure(IConfigurationModule configurationModule)
		{
			Configure(configurationModule.Configure);
		}

		#endregion

		#region CreateContext Methods

		/// <summary>
		/// Create an injection context associated with this scope
		/// </summary>
		/// <returns></returns>
		public IInjectionContext CreateContext(IDisposalScope disposalScope = null)
		{
			if (disposalScope == null)
			{
				return disposalScopeProvider == null
					? new InjectionContext(this, this)
					: new InjectionContext(disposalScopeProvider.ProvideDisposalScope(this), this);
			}

			return new InjectionContext(disposalScope, this);
		}

		#endregion

		#region Locate methods

		/// <summary>
		/// Locate an export by type
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Locate<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
		{
			return (T)Locate(typeof(T), injectionContext, consider, locateKey);
		}

		/// <summary>
		/// Locate an object by type
		/// </summary>
		/// <param name="objectType"></param>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Locate(Type objectType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
		{
			if (objectType == null)
			{
				throw new ArgumentNullException("objectType");
			}

			if (log.IsDebugEnabled)
			{
				log.DebugFormat("Locate by type {0} with injectionContext is null: {1} and consider is null {2}",
					objectType.FullName,
					injectionContext == null,
					consider == null);
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			object returnValue = null;

			try
			{
				ExportStrategyCollection collection;

				if (exportsByType.TryGetValue(objectType, out collection))
				{
					returnValue = collection.Activate(null, objectType, injectionContext, consider, locateKey);

					if (returnValue != null)
					{
						return returnValue;
					}
				}

				if (objectType.IsConstructedGenericType)
				{
					IExportStrategy exportStrategy = GetStrategy(objectType, injectionContext: injectionContext);

					// I'm doing a second look up incase two threads are trying to create a generic at the same exact time
					// and they have a singleton you have to use the same export strategy
					if (exportStrategy != null && exportsByType.TryGetValue(objectType, out collection))
					{
						returnValue = collection.Activate(null, objectType, injectionContext, consider, locateKey);
					}
				}

				ReadOnlyCollection<ISecondaryExportLocator> tempSecondaryResolvers = secondaryResolvers;

				if (returnValue == null && tempSecondaryResolvers != null)
				{
					foreach (ISecondaryExportLocator secondaryDependencyResolver in tempSecondaryResolvers)
					{
						returnValue = secondaryDependencyResolver.Locate(this, injectionContext, null, objectType, consider, locateKey);

						if (returnValue != null)
						{
							break;
						}
					}
				}

				if (returnValue == null && ParentScope != null)
				{
					returnValue = ParentScope.Locate(objectType, injectionContext, consider, locateKey);
				}

				if (returnValue != null || injectionContext.RequestingScope != this)
				{
					return returnValue;
				}

				if (objectType.IsConstructedGenericType)
				{
					returnValue = ProcessSpecialGenericType<object>(injectionContext, objectType, consider, locateKey);
				}
				else if (objectType.IsArray)
				{
					returnValue = ProcessArrayType<object>(injectionContext, objectType, consider, locateKey);
				}
				else if (objectType.GetTypeInfo().BaseType == typeof(MulticastDelegate))
				{
					returnValue = ProcessDelegateType(injectionContext, objectType, consider, locateKey);
				}

				if (returnValue == null)
				{
					returnValue = ProcessICollectionType(injectionContext, objectType, consider, locateKey);
				}

				if (returnValue == null)
				{
					returnValue = ResolveUnknownExport(objectType, null, injectionContext, consider, locateKey);
				}
			}
			catch (LocateException exp)
			{
				exp.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(null, objectType, ScopeName, consider != null, false));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from Locate by type {0} in scope {1} id {2}",
						objectType.FullName,
						ScopeName,
						ScopeId),
					exp);
			}
			catch (Exception exp)
			{
				GeneralLocateException generalLocateException = new GeneralLocateException(null, objectType, injectionContext, exp);

				generalLocateException.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(null, objectType, ScopeName, consider != null, false));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw generalLocateException;
				}

				log.Error(
					string.Format("Exception was thrown from Locate by type {0} in scope {1} id {2}",
						objectType.FullName,
						ScopeName,
						ScopeId),
					exp);
			}

			return returnValue;
		}

		/// <summary>
		/// Locate an export by name
		/// </summary>
		/// <param name="exportName"></param>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Locate(string exportName, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null)
		{
			if (exportName == null)
			{
				throw new ArgumentNullException("exportName");
			}

			exportName = exportName.ToLowerInvariant();

			if (log.IsDebugEnabled)
			{
				log.DebugFormat("Locate by name {0} with injectionContext is null: {1} and consider is null {2}",
					exportName,
					injectionContext == null,
					consider == null);
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			try
			{
				object returnValue = null;

				ExportStrategyCollection collection;

				if (exportsByName.TryGetValue(exportName, out collection))
				{
					returnValue = collection.Activate(exportName, null, injectionContext, consider, locateKey);

					if (returnValue != null)
					{
						return returnValue;
					}
				}

				ReadOnlyCollection<ISecondaryExportLocator> tempSecondaryResolvers = secondaryResolvers;

				if (tempSecondaryResolvers != null)
				{
					foreach (ISecondaryExportLocator secondaryDependencyResolver in tempSecondaryResolvers)
					{
						returnValue = secondaryDependencyResolver.Locate(this, injectionContext, exportName, null, consider, locateKey);

						if (returnValue != null)
						{
							break;
						}
					}
				}

				if (ParentScope != null)
				{
					returnValue = ParentScope.Locate(exportName, injectionContext, consider, locateKey);
				}

				if (returnValue == null && injectionContext.RequestingScope == this)
				{
					returnValue = ResolveUnknownExport(typeof(object), exportName, injectionContext, consider, locateKey);
				}

				if (returnValue != null)
				{
					return returnValue;
				}
			}
			catch (LocateException exp)
			{
				exp.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(exportName, null, ScopeName, consider != null, false));

				if (kernelManager.Container != null &&
					kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from Locate by name {0} in scope {1} id {2}", exportName, ScopeName, ScopeId),
					exp);
			}
			catch (Exception exp)
			{
				GeneralLocateException generalLocateException = new GeneralLocateException(exportName,(Type) null, injectionContext, exp);

				generalLocateException.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(exportName, null, ScopeName, consider != null, false));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw generalLocateException;
				}

				log.Error(
					string.Format("Exception was thrown from Locate by name {0} in scope {1} id {2}", exportName, ScopeName, ScopeId),
					exp);
			}

			return null;
		}

		/// <summary>
		/// Locate all export of type T
		/// </summary>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <param name="comparer"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public List<T> LocateAll<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<T> comparer = null)
		{
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("LocateAll<T> type {0} with injectionContext is null: {1} and consider is null {2}",
					typeof(T).FullName,
					injectionContext == null,
					consider == null);
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			List<T> returnValue;
			try
			{
				returnValue = InternalLocateAllWithContext<T>(injectionContext, typeof(T).FullName, typeof(T), consider, locateKey);
			}
			catch (LocateException exp)
			{
				exp.AddLocationInformationEntry(new InjectionScopeLocateEntry(null, typeof(T), ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll<T> for type {0} in scope {1} id {2}",
						typeof(T).FullName,
						ScopeName,
						ScopeId),
					exp);

				returnValue = new List<T>();

			}
			catch (Exception exp)
			{
				GeneralLocateException generalLocateException = new GeneralLocateException(null, typeof(T), injectionContext, exp);

				generalLocateException.AddLocationInformationEntry(new InjectionScopeLocateEntry(null, typeof(T), ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw generalLocateException;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll<T> for type {0} in scope {1} id {2}",
						typeof(T).FullName,
						ScopeName,
						ScopeId),
					exp);

				returnValue = new List<T>();
			}

			if (comparer != null)
			{
				returnValue.Sort(comparer);
			}

			return returnValue;
		}

		/// <summary>
		/// Locate All exports by the name provided
		/// </summary>
		/// <param name="name"></param>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public List<object> LocateAll(string name, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<object> comparer = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			name = name.ToLowerInvariant();

			if (log.IsDebugEnabled)
			{
				log.DebugFormat("LocateAll by name {0} with injectionContext is null: {1} and consider is null {2}",
					name,
					injectionContext == null,
					consider == null);
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			List<object> returnValue;

			try
			{
				returnValue = InternalLocateAllWithContext<object>(injectionContext, name, null, consider, locateKey);
			}
			catch (LocateException exp)
			{
				exp.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(name, null, ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll by name {0} in scope {1} id {2}", name, ScopeName, ScopeId),
					exp);

				returnValue = new List<object>();
			}
			catch (Exception exp)
			{
				GeneralLocateException generalLocateException = new GeneralLocateException(name,(Type) null, injectionContext, exp);

				generalLocateException.AddLocationInformationEntry(new InjectionScopeLocateEntry(name, null, ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll by name {0} in scope {1} id {2}", name, ScopeName, ScopeId),
					exp);

				returnValue = new List<object>();
			}

			if (comparer != null)
			{
				returnValue.Sort(comparer);
			}

			return returnValue;
		}

		/// <summary>
		/// Locate all exports by type
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="injectionContext"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <param name="comparer1"></param>
		/// <returns></returns>
		public List<object> LocateAll(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object locateKey = null, IComparer<object> comparer1 = null)
		{
			if (exportType == null)
			{
				throw new ArgumentNullException("exportType");
			}

			if (log.IsDebugEnabled)
			{
				log.DebugFormat("LocateAll by type {0} with injectionContext is null: {1} and consider is null {2}",
					exportType.FullName,
					injectionContext == null,
					consider == null);
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			try
			{
				return InternalLocateAllWithContext<object>(injectionContext, exportType.FullName, exportType, consider, locateKey);
			}
			catch (LocateException exp)
			{
				exp.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(null, exportType, ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll by type {0} in scope {1} id {2}",
						exportType.FullName,
						ScopeName,
						ScopeId),
					exp);

				return new List<object>();
			}
			catch (Exception exp)
			{
				GeneralLocateException generalLocateException = new GeneralLocateException(null, exportType, injectionContext, exp);

				generalLocateException.AddLocationInformationEntry(
					new InjectionScopeLocateEntry(null, exportType, ScopeName, consider != null, true));

				if (kernelManager.Container != null &&
					 kernelManager.Container.ThrowExceptions)
				{
					throw generalLocateException;
				}

				log.Error(
					string.Format("Exception was thrown from LocateAll by type {0} in scope {1} id {2}",
						exportType.FullName,
						ScopeName,
						ScopeId),
					exp);

				return new List<object>();
			}
		}

		#endregion

		#region ExtraData Methods

		/// <summary>
		/// Extra data associated with the injection request. 
		/// </summary>
		/// <param name="dataName"></param>
		/// <returns></returns>
		public object GetExtraData(string dataName)
		{
			object returnValue = null;

			if (extraData != null)
			{
				lock (extraDataLock)
				{
					extraData.TryGetValue(dataName, out returnValue);
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
			lock (extraDataLock)
			{
				if (extraData == null)
				{
					extraData = new Dictionary<string, object>();
				}

				extraData[dataName] = newValue;
			}
		}

		#endregion

		#region Get Strategy Methods

		/// <summary>
		/// Returns a list of all known strategies.
		/// </summary>
		/// <param name="exportFilter"></param>
		/// <returns>returns all known strategies</returns>
		public IEnumerable<IExportStrategy> GetAllStrategies(ExportStrategyFilter exportFilter)
		{
			IInjectionContext context = null;
			HashSet<IExportStrategy> returnValue = new HashSet<IExportStrategy>();
			IEnumerable<ExportStrategyCollection> currentExports = exportsByName.Values;

			if (exportFilter != null)
			{
				context = CreateContext();
			}

			foreach (ExportStrategyCollection exportStrategyCollection in currentExports)
			{
				foreach (IExportStrategy exportStrategy in exportStrategyCollection.ExportStrategies)
				{
					if (exportFilter == null || exportFilter(context, exportStrategy))
					{
						returnValue.Add(exportStrategy);
					}
				}
			}

			currentExports = exportsByType.Values;

			foreach (ExportStrategyCollection exportStrategyCollection in currentExports)
			{
				foreach (IExportStrategy exportStrategy in exportStrategyCollection.ExportStrategies)
				{
					if (exportFilter == null || exportFilter(context, exportStrategy))
					{
						returnValue.Add(exportStrategy);
					}
				}
			}

			return returnValue;
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
			ExportStrategyCollection collection;
			IInjectionContext context = injectionContext ?? CreateContext();

			name = name.ToLowerInvariant();

			if (exportsByName.TryGetValue(name, out collection))
			{
				foreach (IExportStrategy exportStrategy in collection.ExportStrategies)
				{
					if (exportStrategy.MeetsCondition(context) &&
						 (exportFilter == null || exportFilter(context, exportStrategy)))
					{
						return exportStrategy;
					}
				}
			}

			return null;
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
			IExportStrategy exportStrategy = null;
			ExportStrategyCollection collection;
			IInjectionContext context = injectionContext ?? CreateContext();

			if (exportsByType.TryGetValue(exportType, out collection))
			{
				foreach (IExportStrategy currentExportStrategy in collection.ExportStrategies)
				{
					if (withKey != null)
					{
						if (withKey is IEnumerable && !(withKey is string))
						{
							foreach (object key in withKey as IEnumerable)
							{
								if (key.Equals(currentExportStrategy.Key))
								{
									return currentExportStrategy;
								}
							}
						}
						else if (withKey.Equals(currentExportStrategy.Key))
						{
							return currentExportStrategy;
						}
					}
					else if (currentExportStrategy.MeetsCondition(context) &&
							  (exportFilter == null || exportFilter(injectionContext, currentExportStrategy)))
					{
						return currentExportStrategy;
					}
				}
			}

			if (exportType.IsConstructedGenericType)
			{
				Type genericType = exportType.GetGenericTypeDefinition();

				if (exportsByType.TryGetValue(genericType, out collection))
				{
					Type[] closingTypes = exportType.GenericTypeArguments;

					foreach (IExportStrategy strategy in collection.ExportStrategies)
					{
						IGenericExportStrategy genericExportStrategy = strategy as IGenericExportStrategy;

						if (genericExportStrategy != null &&
							 genericExportStrategy.MeetsCondition(context) &&
							 (exportFilter == null || exportFilter(context, genericExportStrategy)))
						{
							if (genericExportStrategy.OwningScope != this)
							{
								exportStrategy = genericExportStrategy.OwningScope.GetStrategy(exportType, context, exportFilter, withKey);
							}
							else
							{
								exportStrategy = genericExportStrategy.CreateClosedStrategy(exportType);
							}

							if (exportStrategy != null)
							{
								AddStrategy(exportStrategy);

								break;
							}
						}
					}
				}
			}

			return exportStrategy;
		}

		/// <summary>
		/// Get the list of exported strategies sorted by best option.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> GetStrategies(string name,
			IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter = null)
		{
			ExportStrategyCollection returnValue;
			IInjectionContext context = injectionContext ?? CreateContext();

			name = name.ToLowerInvariant();

			if (exportsByName.TryGetValue(name, out returnValue))
			{
				foreach (IExportStrategy exportStrategy in returnValue.ExportStrategies)
				{
					if (exportStrategy.MeetsCondition(context) &&
						 (exportFilter == null || exportFilter(injectionContext, exportStrategy)))
					{
						yield return exportStrategy;
					}
				}
			}
		}

		/// <summary>
		/// Get the list of exported strategies sorted by best option.
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <returns></returns>
		public IEnumerable<IExportStrategy> GetStrategies(Type exportType,
			IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter = null)
		{
			ExportStrategyCollection returnValue;
			IInjectionContext context = injectionContext ?? CreateContext();

			if (exportsByType.TryGetValue(exportType, out returnValue))
			{
				foreach (IExportStrategy exportStrategy in returnValue.ExportStrategies)
				{
					if (exportStrategy.MeetsCondition(context) &&
						 (exportFilter == null || exportFilter(injectionContext, exportStrategy)))
					{
						yield return exportStrategy;
					}
				}
			}
		}

		/// <summary>
		/// Get the export strategy collection
		/// </summary>
		/// <param name="exportType">type to locate</param>
		/// <returns>can be null if nothing is registered by that name</returns>
		public IExportStrategyCollection GetStrategyCollection(Type exportType)
		{
			ExportStrategyCollection returnValue;

			if (!exportsByType.TryGetValue(exportType, out returnValue) && ParentScope == null)
			{
				lock (exportsLock)
				{
					Dictionary<Type, ExportStrategyCollection> newExports =
						new Dictionary<Type, ExportStrategyCollection>(exportsByType);

					returnValue = new ExportStrategyCollection(this, Environment, comparer);

					newExports[exportType] = returnValue;

					exportsByType = new ReadOnlyDictionary<Type, ExportStrategyCollection>(newExports);
				}
			}

			return returnValue;
		}

		#endregion

		#region Add Remove Methods

		/// <summary>
		/// Adds a new strategy to the container
		/// </summary>
		/// <param name="addStrategy"></param>
		public void AddStrategy(IExportStrategy addStrategy)
		{
			List<IExportStrategy> newExportStrategies = new List<IExportStrategy> { addStrategy };

			addStrategy.OwningScope = this;

			addStrategy.Initialize();

			foreach (IExportStrategy secondaryStrategy in addStrategy.SecondaryStrategies())
			{
				secondaryStrategy.OwningScope = this;

				secondaryStrategy.Initialize();

				newExportStrategies.Add(secondaryStrategy);
			}

			AddExportStrategies(newExportStrategies);
		}

		/// <summary>
		/// Allows the caller to remove a strategy from the container
		/// </summary>
		/// <param name="knownStrategy">strategy to remove</param>
		public void RemoveStrategy(IExportStrategy knownStrategy)
		{
			if (knownStrategy.ExportNames != null)
			{
				foreach (string exportName in knownStrategy.ExportNames)
				{
					ExportStrategyCollection collection;

					if (exportsByName.TryGetValue(exportName, out collection))
					{
						collection.RemoveExport(knownStrategy);
					}
				}
			}

			if (knownStrategy.ExportTypes != null)
			{
				foreach (Type exportType in knownStrategy.ExportTypes)
				{
					ExportStrategyCollection collection;

					if (exportsByType.TryGetValue(exportType, out collection))
					{
						collection.RemoveExport(knownStrategy);
					}
				}
			}
		}

		/// <summary>
		/// Inject dependencies into a constructed object
		/// </summary>
		/// <param name="injectedObject">object to be injected</param>
		/// <param name="injectionContext">injection context</param>
		public void Inject(object injectedObject, IInjectionContext injectionContext = null)
		{
			if (injectedObject == null)
			{
				throw new ArgumentNullException("injectedObject");
			}

			if (injectionContext == null)
			{
				injectionContext = CreateContext();
			}

			IInjectionStrategy injectionStrategy;

			if (injections.TryGetValue(injectedObject.GetType(), out injectionStrategy))
			{
				injectionStrategy.Inject(injectionContext, injectedObject);
			}
			else if (ParentScope != null)
			{
				ParentScope.Inject(injectedObject, injectionContext);
			}
			else
			{
				AttributedInjectionStrategy attributedInjectionStrategy = new AttributedInjectionStrategy(injectedObject.GetType());

				attributedInjectionStrategy.Initialize();

				lock (exportsLock)
				{
					Dictionary<Type, IInjectionStrategy> newInjectionStrategies = new Dictionary<Type, IInjectionStrategy>(injections);

					newInjectionStrategies[injectedObject.GetType()] = attributedInjectionStrategy;

					injections = new ReadOnlyDictionary<Type, IInjectionStrategy>(newInjectionStrategies);
				}

				attributedInjectionStrategy.Inject(injectionContext, injectedObject);
			}
		}

		#endregion

		#region LocateMissingExport

		/// <summary>
		/// Locate missing exports, this is an internal method
		/// </summary>
		/// <param name="context"></param>
		/// <param name="exportName"></param>
		/// <param name="exportType"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object LocateMissingExport(IInjectionContext context,
			string exportName,
			Type exportType,
			ExportStrategyFilter consider,
			object locateKey)
		{
			// skip trynig to locate,only do this from root scope
			if (ParentScope != null)
			{
				return null;
			}

			ReadOnlyCollection<ISecondaryExportLocator> tempSecondaryResolvers = secondaryResolvers;

			// loop through the list of secondary resolvers because this method is only called from ExportStrategyCollection
			// this route will not 
			if (tempSecondaryResolvers != null)
			{
				foreach (ISecondaryExportLocator secondaryDependencyResolver in tempSecondaryResolvers)
				{
					object returnValue = secondaryDependencyResolver.Locate(this, context, exportName, exportType, consider, locateKey);

					if (returnValue != null)
					{
						return returnValue;
					}
				}
			}

			return ResolveUnknownExport(exportType, exportName, context, consider, locateKey);
		}

		#endregion

		#region ImportTypeByName

		/// <summary>
		/// True if the type should be imported by name rather than type
		/// </summary>
		/// <param name="importType"></param>
		/// <returns></returns>
		public static bool ImportTypeByName(Type importType)
		{
			return importType.GetTypeInfo().IsPrimitive ||
					 importType.GetTypeInfo().IsEnum ||
					 importType == typeof(string) ||
					 importType == typeof(decimal) ||
					 importType == typeof(DateTime) ||
					 importType == typeof(DateTimeOffset) ||
					 importType == typeof(TimeSpan) ||
					 importType == typeof(Guid);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Dispose of this kernel and child kernels
		/// </summary>
		/// <param name="dispose"></param>
		protected override void Dispose(bool dispose)
		{
			if (disposed)
			{
				return;
			}

			if (dispose)
			{
				foreach (ExportStrategyCollection exportStrategyCollection in exportsByName.Values)
				{
					exportStrategyCollection.Dispose();
				}

				foreach (ExportStrategyCollection exportStrategyCollection in exportsByType.Values)
				{
					exportStrategyCollection.Dispose();
				}

				exportsByName = null;
				exportsByType = null;

				base.Dispose(true);
			}
		}

		#endregion

		#region Private Methods

		private object ResolveUnknownExport(Type resolveType,
			string resolveName,
			IInjectionContext injectionContext,
			ExportStrategyFilter consider,
			object locateKey)
		{
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("Resolving unknown export with type '{0}' and name '{1}'",
					resolveType != null ? resolveType.FullName : string.Empty,
					resolveName);
			}

			object returnValue = null;

			if (kernelManager.Container != null)
			{
				returnValue = kernelManager.Container.LocateMissingExport(injectionContext, resolveName, resolveType, consider, locateKey);

				if (returnValue == null &&
					 kernelManager.Container.AutoRegisterUnknown &&
					 resolveType != null &&
					 string.IsNullOrEmpty(resolveName) &&
					 !resolveType.GetTypeInfo().IsAbstract &&
					 !resolveType.GetTypeInfo().IsGenericTypeDefinition)
				{
					ConcreteAttributeExportStrategy strategy =
						new ConcreteAttributeExportStrategy(resolveType,
							resolveType.GetTypeInfo().GetCustomAttributes());

					strategy.AddExportType(resolveType);

					IInjectionScope addScope = this;

					while (addScope.ParentScope != null)
					{
						addScope = addScope.ParentScope;
					}

					addScope.AddStrategy(strategy);

					return strategy.Activate(this, injectionContext, consider, locateKey);
				}
			}

			return returnValue;
		}

		private object ProcessSpecialGenericType<T>(IInjectionContext injectionContext, Type type, ExportStrategyFilter consider, object locateKey)
		{
			Type openGenericType = type.GetGenericTypeDefinition();
			Type exportStrategyType;

			if (type == typeof(Func<Type, object>))
			{
				return new Func<Type, object>(inType => Locate(inType, null, consider, locateKey));
			}

			if (type == typeof(Func<Type, IInjectionContext, object>))
			{
				return new Func<Type, IInjectionContext, object>((inType, context) => Locate(inType, context, consider, locateKey));
			}

			if (openGenericStrategyMapping.TryGetValue(openGenericType, out exportStrategyType))
			{
				Type closedExportStrategyType = exportStrategyType.MakeGenericType(type.GenericTypeArguments);

				IExportStrategy exportStrategy = (IExportStrategy)Activator.CreateInstance(closedExportStrategyType);

				AddStrategy(exportStrategy);

				return exportStrategy.Activate(this, injectionContext, consider, locateKey);
			}

			return null;
		}

		private object ProcessDelegateType(IInjectionContext injectionContext, Type objectType, ExportStrategyFilter consider, object locateKey)
		{
			object returnValue = null;
			MethodInfo invokeInfo = objectType.GetTypeInfo().GetDeclaredMethod("Invoke");

			if (invokeInfo.ReturnType != typeof(void))
			{
				ParameterInfo[] parameterInfos = invokeInfo.GetParameters().ToArray();
				Type openType = null;

				switch (parameterInfos.Length)
				{
					case 0:
						openType = typeof(GenericDelegateExportStrategy<,>);
						break;
					case 1:
						openType = typeof(GenericDelegateExportStrategy<,,>);
						break;
					case 2:
						openType = typeof(GenericDelegateExportStrategy<,,,>);
						break;
					case 3:
						openType = typeof(GenericDelegateExportStrategy<,,,,>);
						break;
					case 4:
						openType = typeof(GenericDelegateExportStrategy<,,,,,>);
						break;
					case 5:
						openType = typeof(GenericDelegateExportStrategy<,,,,,,>);
						break;
				}

				if (openType != null)
				{
					List<Type> closeList = new List<Type>
					                       {
						                       objectType,
						                       invokeInfo.ReturnType
					                       };

					closeList.AddRange(parameterInfos.Select(x => x.ParameterType));

					Type closedType = openType.MakeGenericType(closeList.ToArray());

					IExportStrategy exportStrategy = Activator.CreateInstance(closedType) as IExportStrategy;

					if (exportStrategy != null)
					{
						AddStrategy(exportStrategy);

						returnValue = exportStrategy.Activate(this, injectionContext, consider, locateKey);
					}
				}
			}

			return returnValue;
		}


		private object ProcessArrayType<T>(IInjectionContext injectionContext, Type tType, ExportStrategyFilter consider, object locateKey)
		{
			Type arrayStrategyType = typeof(ArrayExportStrategy<>).MakeGenericType(tType.GetElementType());

			IExportStrategy exportStrategy = (IExportStrategy)Activator.CreateInstance(arrayStrategyType);

			AddStrategy(exportStrategy);

			return exportStrategy.Activate(this, injectionContext, consider, locateKey);
		}

		private object ProcessICollectionType(IInjectionContext injectionContext, Type tType, ExportStrategyFilter consider, object locateKey)
		{
			Type icollectionType =
				tType.GetTypeInfo()
					.ImplementedInterfaces.FirstOrDefault(
						t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>));

			if (icollectionType != null)
			{
				Type itemType = icollectionType.GenericTypeArguments[0];

				Type collectionStrategyType = typeof(ICollectionExportStrategy<,>).MakeGenericType(tType, itemType);

				IExportStrategy exportStrategy = (IExportStrategy)Activator.CreateInstance(collectionStrategyType);

				AddStrategy(exportStrategy);

				return exportStrategy.Activate(this, injectionContext, consider, locateKey);
			}

			return null;
		}

		private List<T> InternalLocateAllWithContext<T>(IInjectionContext injectionContext, string name, Type locateType, ExportStrategyFilter exportFilter, object locateKey)
		{
			List<T> returnValue = new List<T>();
			bool specialType = false;

			if (locateType != null && locateType.IsConstructedGenericType)
			{
				Type genericType = locateType.GetGenericTypeDefinition();
				Type[] genericArgs = locateType.GetTypeInfo().GenericTypeArguments;

				CheckForNewGenericExports<T>(injectionContext, locateType, genericType, genericArgs);

				if (genericType == typeof(Lazy<>))
				{
					MethodInfo methodInfo = locateLazyListMethodInfo.MakeGenericMethod(locateType, genericArgs[0]);

					methodInfo.Invoke(this, new[] { injectionContext, exportFilter, locateKey, returnValue });

					specialType = true;
				}
				else if (genericType == typeof(Owned<>))
				{
					MethodInfo methodInfo = locateOwnedListMethodInfo.MakeGenericMethod(locateType, genericArgs[0]);

					methodInfo.Invoke(this, new[] { injectionContext, exportFilter, locateKey, returnValue });

					specialType = true;
				}
				else if (genericType == typeof(Meta<>))
				{
					MethodInfo methodInfo = locateMetaListMethodInfo.MakeGenericMethod(locateType, genericArgs[0]);

					methodInfo.Invoke(this, new[] { injectionContext, exportFilter, locateKey, returnValue });

					specialType = true;
				}
			}

			if (!specialType)
			{
				ExportStrategyCollection exportStrategyCollection;

				if (locateType != null)
				{
					if (exportsByType.TryGetValue(locateType, out exportStrategyCollection))
					{
						returnValue.AddRange(exportStrategyCollection.ActivateAll<T>(injectionContext, exportFilter, locateKey));
					}
				}
				else if (exportsByName.TryGetValue(name, out exportStrategyCollection))
				{
					returnValue.AddRange(exportStrategyCollection.ActivateAll<T>(injectionContext, exportFilter, locateKey));
				}
			}

			ReadOnlyCollection<ISecondaryExportLocator> tempResolvers = secondaryResolvers;

			if (tempResolvers != null)
			{
				foreach (ISecondaryExportLocator secondaryDependencyResolver in tempResolvers)
				{
					IEnumerable<object> locatedObjects =
						secondaryDependencyResolver.LocateAll(this,
																			injectionContext,
																			name,
																			locateType,
																			returnValue.Count == 0,
																			exportFilter,
																			locateKey);

					foreach (object locatedObject in locatedObjects)
					{
						returnValue.Add((T)locatedObject);
					}
				}
			}

			if (ParentScope != null)
			{
				if (locateType != null)
				{
					foreach (T t in ParentScope.LocateAll(locateType, injectionContext, exportFilter, locateKey))
					{
						returnValue.Add(t);
					}
				}
				else
				{
					foreach (T t in ParentScope.LocateAll(name, injectionContext, exportFilter, locateKey))
					{
						returnValue.Add(t);
					}
				}
			}

			return returnValue;
		}

		private void CheckForNewGenericExports<T>(IInjectionContext injectionContext,
			Type locateType,
			Type genericType,
			Type[] genericArgs)
		{
			if (locateType != null && locateType.IsConstructedGenericType)
			{
				ExportStrategyCollection genericCollection = null;

				if (exportsByType.TryGetValue(genericType, out genericCollection))
				{
					List<IExportStrategy> strategies = new List<IExportStrategy>(genericCollection.ExportStrategies);

					ExportStrategyCollection exportStrategyCollection;

					if (exportsByType.TryGetValue(locateType, out exportStrategyCollection))
					{
						foreach (IExportStrategy exportStrategy in exportStrategyCollection.ExportStrategies)
						{
							ICompiledExportStrategy compiledExport = exportStrategy as ICompiledExportStrategy;

							if (compiledExport != null && compiledExport.CreatingStrategy != null)
							{
								strategies.Remove(compiledExport.CreatingStrategy);
							}
						}
					}

					foreach (IExportStrategy exportStrategy in strategies)
					{
						GenericExportStrategy genericExportStrategy = exportStrategy as GenericExportStrategy;

						if (genericExportStrategy != null &&
							 genericExportStrategy.MeetsCondition(injectionContext))
						{
							IExportStrategy newStrategy =
								genericExportStrategy.CreateClosedStrategy(locateType);

							if (newStrategy != null)
							{
								AddStrategy(newStrategy);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Locates a list of Lazy&lt;T&gt;
		/// </summary>
		/// <typeparam name="TLazy">type of lazy T</typeparam>
		/// <typeparam name="T">type to resolve</typeparam>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportFilter">export filter to apply</param>
		/// <param name="locateKey"></param>
		/// <param name="returnList">return list</param>
		protected void LocateListOfLazyExports<TLazy, T>(IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter,
			object locateKey,
			List<TLazy> returnList) where TLazy : Lazy<T>
		{
			ExportStrategyCollection collection;

			if (exportsByType.TryGetValue(typeof(T), out collection))
			{
				returnList.AddRange(collection.ActivateAllLazy<TLazy, T>(injectionContext, exportFilter, locateKey));
			}
		}

		/// <summary>
		/// Locates a list of Owned&lt;T&gt;
		/// </summary>
		/// <typeparam name="TOwned">type of owned T</typeparam>
		/// <typeparam name="T">type to located</typeparam>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportFilter">export filter to apply</param>
		/// <param name="locateKey"></param>
		/// <param name="returnList">list to return</param>
		protected void LocateListOfOwnedExports<TOwned, T>(IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter,
			object locateKey,
			List<TOwned> returnList)
			where TOwned : Owned<T>
			where T : class
		{
			ExportStrategyCollection collection;

			if (exportsByType.TryGetValue(typeof(T), out collection))
			{
				returnList.AddRange(collection.ActivateAllOwned<TOwned, T>(injectionContext, exportFilter, locateKey));
			}
		}

		/// <summary>
		/// Locate a list of Meta&lt;T&gt;
		/// </summary>
		/// <typeparam name="TMeta">type of meta object</typeparam>
		/// <typeparam name="T">type to locate</typeparam>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportFilter">export filter</param>
		/// <param name="locateKey"></param>
		/// <param name="returnList">list to populate</param>
		protected void LocateListOfMetaExports<TMeta, T>(IInjectionContext injectionContext,
			ExportStrategyFilter exportFilter,
			object locateKey,
			List<TMeta> returnList) where TMeta : Meta<T>
		{
			ExportStrategyCollection collection;

			if (exportsByType.TryGetValue(typeof(T), out collection))
			{
				returnList.AddRange(collection.ActivateAllMeta<TMeta, T>(injectionContext, exportFilter, locateKey));
			}
		}

		private string DebugDisplayString
		{
			get { return "Exports: " + GetAllStrategies(null).Count(); }
		}

		#endregion
	}
}