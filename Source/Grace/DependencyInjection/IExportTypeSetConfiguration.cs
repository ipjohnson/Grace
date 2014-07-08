using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Allows you to configure an assembly for export
	/// </summary>
	public interface IExportTypeSetConfiguration
	{
		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition">condition for the export</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration AndCondition(IExportCondition condition);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionFunc">condition for the export</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration AndCondition(Func<Type, IExportCondition> conditionFunc);

		/// <summary>
		/// Export services as Singletons
		/// </summary>
        /// <returns>configuration object</returns>
        [Obsolete("Please use Lifestyle.Singleton()")]
		IExportTypeSetConfiguration AndSingleton();

		/// <summary>
		/// Exports are to be marked as shared, similar to a singleton only using a weak reference.
		/// It can not be of type IDisposable
		/// </summary>
        /// <returns>configuration object</returns>
        [Obsolete("Please use Lifestyle.WeakSingleton()")]
		IExportTypeSetConfiguration AndWeakSingleton();

		/// <summary>
		/// Export all types based on speficied type by Type
		/// </summary>
		/// <param name="baseType">base type to export</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration BasedOn(Type baseType);

		/// <summary>
		/// Export all types based on speficied type by Type
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration BasedOn<T>();

		/// <summary>
		/// Export all objects that implements the specified interface
		/// </summary>
		/// <param name="interfaceType">interface type</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByInterface(Type interfaceType);

		/// <summary>
		/// Export all objects that implements the specified interface
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByInterface<T>();

		/// <summary>
		/// Export all classes by interface or that match a set of interfaces
		/// </summary>
		/// <param name="whereClause">where clause to test if the interface should be used for exporting</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> whereClause = null);

		/// <summary>
		/// Export the selected classes by type
		/// </summary>
		/// <param name="typeDelegate">type delegate to pick what type to export as</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByType(Func<Type, Type> typeDelegate = null);

        /// <summary>
        /// Exports by a set of types
        /// </summary>
        /// <param name="typeDelegate"></param>
        /// <returns></returns>
        IExportTypeSetConfiguration ByTypes(Func<Type, IEnumerable<Type>> typeDelegate);

		/// <summary>
		/// Export by a particular name 
		/// </summary>
		/// <param name="nameDelegate">delegate used to create export name, default is type => type.Name</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByName(Func<Type, string> nameDelegate = null);

		/// <summary>
		/// Enrich all with a particular delegate
		/// </summary>
		/// <param name="enrichWithDelegate">enrichment delegate</param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate);

		/// <summary>
		/// Enrich all with a particular delegate
		/// </summary>
		/// <param name="enrichWithDelegates">enrichment delegates, IEnumerable must not be null</param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWith(Func<Type, IEnumerable<EnrichWithDelegate>> enrichWithDelegates);

		/// <summary>
		/// Enrich all with linq expressions
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider);

		/// <summary>
		/// Enrich all with linq expressions
		/// </summary>
		/// <param name="providers">expression provider, IEnumerable must not be null</param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWithExpression(Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>> providers);

		/// <summary>
		/// Exclude a type from being used
		/// </summary>
		/// <param name="exclude">exclude delegate</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude);

		/// <summary>
		/// Export all attributed types
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ExportAttributedTypes();

		/// <summary>
		/// Mark the exports to be externally owned, stopping the container from calling the Dispose
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ExternallyOwned();

		/// <summary>
		/// Import properties of type TProperty and by name
		/// </summary>
		/// <typeparam name="TProperty">property type</typeparam>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration ImportProperty<TProperty>();

		/// <summary>
		/// Import all properties that match the type
		/// </summary>
		/// <param name="propertyType"></param>
		/// <returns></returns>
		IExportTypeSetImportPropertyConfiguration ImportProperty(Type propertyType);

		/// <summary>
		/// Export in the specified Environment
		/// </summary>
		/// <param name="environment">environment to export in</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration InEnvironment(ExportEnvironment environment);

        /// <summary>
        /// Assign a lifestyle to all exports
        /// </summary>
        LifestyleBulkConfiguration Lifestyle { get; }

		/// <summary>
		/// Allows you to filter out types based on the provided where clause
		/// </summary>
		/// <param name="whereClause">where clause</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration Select(Func<Type, bool> whereClause);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">condition delegate</param>
		/// /// <returns>configuration object</returns>
		IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate);


        /// <summary>
        /// Set a particular life style
        /// </summary>
        /// <param name="container">lifestyle</param>
        /// <returns>configuration object</returns>
        IExportTypeSetConfiguration UsingLifestyle(ILifestyle container);

        /// <summary>
        /// Set a particular life style using a func
        /// </summary>
        /// <param name="lifestyleFunc">pick a lifestyle</param>
        /// <returns>configuration object</returns>
        IExportTypeSetConfiguration UsingLifestyle(Func<Type, ILifestyle> lifestyleFunc);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">when condition</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate);

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <typeparam name="TParam">constructor param type</typeparam>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
	    IExportTypeSetCtorParamConfiguration WithCtorParam<TParam>(Func<TParam> paramFunc = null);

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <typeparam name="TParam">constructor param type</typeparam>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration WithCtorParam<TParam>(Func<IInjectionScope, IInjectionContext, TParam> paramFunc);

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <param name="paramType">constructor parameter type</param>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
	    IExportTypeSetCtorParamConfiguration WithCtorParam(Type paramType, Func<IInjectionScope, IInjectionContext, object> paramFunc);

        /// <summary>
        /// Provide a func that will be used to create a key that will be used to register
        /// </summary>
        /// <param name="withKeyFunc">key func</param>
        /// <returns>configuration object</returns>
        IExportTypeSetConfiguration WithKey(Func<Type, object> withKeyFunc);

		/// <summary>
		/// Set a particular life style
		/// </summary>
		/// <param name="container">lifestyle</param>
		/// <returns>configuration object</returns>
		[Obsolete("Please use UsingLifestyle")]
		IExportTypeSetConfiguration WithLifestyle(ILifestyle container);

		/// <summary>
		/// Set a particular life style using a func
		/// </summary>
		/// <param name="lifestyleFunc">pick a lifestyle</param>
        /// <returns>configuration object</returns>
        [Obsolete("Please use UsingLifestyle")]
		IExportTypeSetConfiguration WithLifestyle(Func<Type, ILifestyle> lifestyleFunc);


		/// <summary>
		/// Export with the spcified priority
		/// </summary>
		/// <param name="priority">priority to export at</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithPriority(int priority);

		/// <summary>
		/// Set priority based on a func
		/// </summary>
		/// <param name="priorityFunc"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration WithPriority(Func<Type, int> priorityFunc);

		/// <summary>
		/// Allows you to specify an attribute that will be used to apply 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithPriorityAttribute<T>() where T : Attribute, IExportPriorityAttribute;

		/// <summary>
		/// Inspectors will be called for every export strategy that is found
		/// </summary>
		/// <param name="inspector">inspector object</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithInspector(IExportStrategyInspector inspector);
	}

	/// <summary>
	/// Configuration object 
	/// </summary>
	public interface IExportTypeSetImportPropertyConfiguration : IExportTypeSetConfiguration
	{
		/// <summary>
		/// Property Name to import
		/// </summary>
		/// <param name="propertyName">property name</param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration Named(string propertyName);

		/// <summary>
		/// Is it required
		/// </summary>
		/// <param name="value">is required</param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration IsRequired(bool value);

		/// <summary>
		/// Apply delegate to choose export
		/// </summary>
		/// <param name="consider">consider filter</param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Using Value provider
		/// </summary>
		/// <param name="activationDelegate"></param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration UsingValue(ExportActivationDelegate activationDelegate);

		/// <summary>
		/// Use value provider
		/// </summary>
		/// <param name="valueProvider">value provider</param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration UsingValueProvider(IExportValueProvider valueProvider);

		/// <summary>
		/// Import the property after the instance has been constructed.
		/// The Instance property on IInjectionContext will be populated
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration AfterConstruction();

		/// <summary>
		/// Only import on certain types
		/// </summary>
		/// <param name="filter">type filter</param>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration OnlyOn(Func<Type, bool> filter);
	}
}