using System;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Allows you to configure an assembly for export
	/// </summary>
	public interface IExportTypeSetConfiguration
	{
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
		/// <param name="whereClause"></param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> whereClause = null);

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <param name="baseType">base type to export</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration BasedOn(Type baseType);

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration BasedOn<T>();

		/// <summary>
		/// Export with the spcified priority
		/// </summary>
		/// <param name="priority">priority to export at</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithPriority(int priority);

		/// <summary>
		/// Allows you to specify an attribute that will be used to apply 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithPriorityAttribute<T>() where T : Attribute, IExportPriorityAttribute;

		/// <summary>
		/// Export in the specified Environment
		/// </summary>
		/// <param name="environment">environment to export in</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Mark the exports to be externally owned, stopping the container from calling the Dispose
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ExternallyOwned();

		/// <summary>
		/// Exports are to be marked as shared, similar to a singleton only using a weak reference.
		/// It can not be of type IDisposable
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration AndWeakSingleton();

		/// <summary>
		/// Export services as Singletons
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration AndSingleton();

		/// <summary>
		/// Set a particular life style
		/// </summary>
		/// <param name="container">lifestyle</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithLifestyle(ILifestyle container);

		/// <summary>
		/// Export all attributed types
		/// </summary>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration ExportAttributedTypes();

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">when condition</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">condition delegate</param>
		/// /// <returns>configuration object</returns>
		IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition">condition for the export</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration AndCondition(IExportCondition condition);

		/// <summary>
		/// Exclude a type from being used
		/// </summary>
		/// <param name="exclude">exclude delegate</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude);

		/// <summary>
		/// Allows you to filter out types based on the provided where clause
		/// </summary>
		/// <param name="whereClause">where clause</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration Select(Func<Type, bool> whereClause);

		/// <summary>
		/// Inspectors will be called for every export strategy that is found
		/// </summary>
		/// <param name="inspector">inspector object</param>
		/// <returns>configuration object</returns>
		IExportTypeSetConfiguration WithInspector(IExportStrategyInspector inspector);

		/// <summary>
		/// Enrich all with a particular delegate
		/// </summary>
		/// <param name="enrichWithDelegate">enrichment delegate</param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate);

		/// <summary>
		/// Enrich all with linq expressions
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider);

		/// <summary>
		/// Import properties of type TProperty and by name
		/// </summary>
		/// <typeparam name="TProperty">property type</typeparam>
		/// <returns>configuration object</returns>
		IExportTypeSetImportPropertyConfiguration ImportProperty<TProperty>();

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
		/// <returns></returns>
		IExportTypeSetImportPropertyConfiguration AfterConstruction();
	}
}