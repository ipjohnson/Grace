using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// This interface can be used to locate an export. It's used by IInjectionScope and IDependencyInjectionContainer
	/// </summary>
	public interface IExportLocator
	{
		/// <summary>
		/// Unique identifier for the instance of the injection scope
		/// </summary>
		Guid ScopeId { get; }

		/// <summary>
		/// The scopes name
		/// </summary>
		[CanBeNull]
		string ScopeName { get; }

		/// <summary>
		/// Adds a secondary resolver to the container.
		/// </summary>
		/// <param name="newLocator">new secondary locator</param>
		void AddSecondaryLocator([NotNull] ISecondaryExportLocator newLocator);

		/// <summary>
		/// List of Export Locators
		/// </summary>
		IEnumerable<ISecondaryExportLocator> SecondaryExportLocators { get; }

		/// <summary>
		/// Add a strategy 
		/// </summary>
		/// <param name="inspector">strategy inspector</param>
        void AddStrategyInspector([NotNull] IExportStrategyInspector inspector);

		/// <summary>
		/// Creates a child scope from this scope
		/// </summary>
		/// <param name="scopeName">name of the scope you want to create</param>
		/// <param name="registrationDelegate">delegate used to configure the new child scope</param>
		/// <param name="disposalScopeProvider">new disposal scope provider for the child scope</param>
		/// <returns>new child scope</returns>
		[NotNull]
		IInjectionScope CreateChildScope(ExportRegistrationDelegate registrationDelegate = null,
													string scopeName = null,
													IDisposalScopeProvider disposalScopeProvider = null);

		/// <summary>
		/// Creates a child scope from this scope using a configuration module
		/// </summary>
		/// <param name="scopeName">name of the scope you want to create</param>
		/// <param name="configurationModule">configuration module used to configure the new child scope</param>
		/// <param name="disposalScopeProvider">new disposal scope for the child scope</param>
		/// <returns>new child scope</returns>
		[NotNull]
		IInjectionScope CreateChildScope([NotNull] IConfigurationModule configurationModule,
													string scopeName = null,
													IDisposalScopeProvider disposalScopeProvider = null);

		/// <summary>
		/// This method can be used to configure the root scope of the container
		/// </summary>
		/// <param name="registrationDelegate">registration delegate used to configure the locator</param>
		void Configure([NotNull] ExportRegistrationDelegate registrationDelegate);

		/// <summary>
		/// This method can be used to configure the root scope of the container
		/// </summary>
		/// <param name="configurationModule">configuration module used to configure the locator</param>
		void Configure([NotNull] IConfigurationModule configurationModule);

		/// <summary>
		/// Create an injection context
		/// </summary>
		/// <returns>new injection context</returns>
		[NotNull]
		IInjectionContext CreateContext(IDisposalScope disposalScope = null);

		/// <summary>
		/// Locate an export by type
		/// </summary>
		/// <param name="injectionContext">injection context for the locate</param>
		/// <param name="consider">filter to be used when locating</param>
		/// <param name="withKey"></param>
		/// <typeparam name="T">type to locate</typeparam>
		/// <returns>export T if found, other wise default(T)</returns>
		T Locate<T>(IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null);

		/// <summary>
		/// Locate an object by type
		/// </summary>
		/// <param name="objectType">type to locate</param>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating export</param>
		/// <param name="withKey"></param>
		/// <returns>export object if found, other wise null</returns>
		object Locate([NotNull] Type objectType,
						  IInjectionContext injectionContext = null,
						  ExportStrategyFilter consider = null,
						  object withKey = null);

		/// <summary>
		/// Locate an export by name
		/// </summary>
		/// <param name="exportName">name of export to locate</param>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <returns>export object if found, other wise null</returns>
		object Locate([NotNull] string exportName,
						  IInjectionContext injectionContext = null,
						  ExportStrategyFilter consider = null,
						  object withKey = null);

		/// <summary>
		/// Locate all export of type T
		/// </summary>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <param name="comparer">used for sorting the imports when returning the list</param>
		/// <typeparam name="T">type to locate</typeparam>
		/// <returns>List of T, this will return an empty list if not exports are found</returns>
		[NotNull]
		List<T> LocateAll<T>(IInjectionContext injectionContext = null,
									ExportStrategyFilter consider = null,
									object withKey = null,
									IComparer<T> comparer = null);

		/// <summary>
		/// Locate All exports by the name provided
		/// </summary>
		/// <param name="name">export name to locate</param>
		/// <param name="injectionContext">injection context to use while locating</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <param name="comparer"></param>
		/// <returns>List of objects, this will return an empty list if no exports are found</returns>
		[NotNull]
		List<object> LocateAll([NotNull] string name,
									  IInjectionContext injectionContext = null,
									  ExportStrategyFilter consider = null,
									  object withKey = null,
									  IComparer<object> comparer = null);

		/// <summary>
		/// Locate all exports by type
		/// </summary>
		/// <param name="exportType">type to locate</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="consider">filter to use while locating</param>
		/// <param name="withKey"></param>
		/// <param name="comparer"></param>
		/// <returns>list of object, this will return an empty list if no exports are found</returns>
		[NotNull]
		List<object> LocateAll(Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter consider = null, object withKey = null, IComparer<object> comparer = null);
        
		/// <summary>
		/// Returns a list of all known strategies.
		/// </summary>
		/// <param name="exportFilter"></param>
		/// <returns>returns all known strategies</returns>
		IEnumerable<IExportStrategy> GetAllStrategies(ExportStrategyFilter exportFilter = null);

		/// <summary>
		/// Finds the best matching strategy exported by the name provided
		/// </summary>
		/// <param name="name"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <param name="withKey"></param>
		/// <returns></returns>
		IExportStrategy GetStrategy(string name, IInjectionContext injectionContext = null, ExportStrategyFilter exportFilter = null, object withKey = null);

		/// <summary>
		/// Finds the best matching strategy exported by the name provided
		/// </summary>
		/// <param name="exportType">type to locate</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="exportFilter">export filter</param>
		/// <param name="withKey"></param>
		/// <returns></returns>
		[CanBeNull]
		IExportStrategy GetStrategy([NotNull] Type exportType, IInjectionContext injectionContext = null, ExportStrategyFilter exportFilter = null, object withKey = null);

		/// <summary>
		/// Get the list of exported strategies sorted by best option.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <returns></returns>
		[NotNull]
		IEnumerable<IExportStrategy> GetStrategies([NotNull] string name,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter exportFilter = null);

		/// <summary>
		/// Get the list of exported strategies sorted by best option.
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportFilter"></param>
		/// <returns></returns>
		[NotNull]
		IEnumerable<IExportStrategy> GetStrategies([NotNull] Type exportType,
			IInjectionContext injectionContext = null,
			ExportStrategyFilter exportFilter = null);

	    /// <summary>
	    /// Get the export strategy collection
	    /// </summary>
	    /// <param name="exportType"></param>
	    /// <param name="createIfDoesntExist"></param>
	    /// <returns>can be null if nothing is registered by that name</returns>
	    [CanBeNull]
		IExportStrategyCollection GetStrategyCollection([NotNull] Type exportType, bool createIfDoesntExist = true);

	    /// <summary>
	    /// Get the export collection by name
	    /// </summary>
	    /// <param name="exportName">export name</param>
	    /// <param name="createIfDoesntExist"></param>
	    /// <returns></returns>
	    [CanBeNull]
        IExportStrategyCollection GetStrategyCollection([NotNull] string exportName, bool createIfDoesntExist = true);

		/// <summary>
		/// Adds a new strategy to the container
		/// </summary>
		/// <param name="addStrategy"></param>
		void AddStrategy([NotNull] IExportStrategy addStrategy);

		/// <summary>
		/// Allows the caller to remove a strategy from the container
		/// </summary>
		/// <param name="knownStrategy">strategy to remove</param>
		void RemoveStrategy([NotNull] IExportStrategy knownStrategy);

		/// <summary>
		/// Inject dependencies into a constructed object
		/// </summary>
		/// <param name="injectedObject">object to be injected</param>
		/// <param name="injectionContext">injection context</param>
		void Inject([NotNull] object injectedObject, IInjectionContext injectionContext = null);


        /// <summary>
        /// List of Injection Inspectors for the scope
        /// </summary>
        [NotNull]
        IEnumerable<IExportStrategyInspector> Inspectors { get; }
        
        /// <summary>
        /// Missing export strategy providers can provide a set of exports that can be used to resolve a satisfy an import
        /// </summary>
        /// <param name="exportStrategyProvider">export strategy provider</param>
        void AddMissingExportStrategyProvider(IMissingExportStrategyProvider exportStrategyProvider);

	}
}