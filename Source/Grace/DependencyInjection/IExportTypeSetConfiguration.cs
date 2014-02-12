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
		/// <returns>returns self</returns>
		IExportTypeSetConfiguration ByInterface(Type interfaceType);

		/// <summary>
		/// Export all objects that implements the specified interface
		/// </summary>
		/// <returns>returns self</returns>
		IExportTypeSetConfiguration ByInterface<T>();

		/// <summary>
		/// Export all classes by interface or that match a set of interfaces
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> whereClause = null);

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <param name="baseType">base type to export</param>
		/// <returns></returns>
		IExportTypeSetConfiguration BasedOn(Type baseType);

		/// <summary>
		/// Export all types based on speficied type
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration BasedOn<T>();

		/// <summary>
		/// Export with the spcified priority
		/// </summary>
		/// <param name="priority">priority to export at</param>
		/// <returns></returns>
		IExportTypeSetConfiguration WithPriority(int priority);

		/// <summary>
		/// Allows you to specify an attribute that will be used to apply 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IExportTypeSetConfiguration WithPriorityAttribute<T>() where T : Attribute, IExportPriorityAttribute;

		/// <summary>
		/// Export in the specified Environment
		/// </summary>
		/// <param name="environment">environment to export in</param>
		/// <returns></returns>
		IExportTypeSetConfiguration InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Mark the exports to be externally owned, stopping the container from calling the Dispose
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration ExternallyOwned();

		/// <summary>
		/// Exports are to be marked as shared, similar to a singleton only using a weak reference.
		/// It can not be of type IDisposable
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration AndWeakSingleton();

		/// <summary>
		/// Export services as Singletons
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration AndSingleton();

		/// <summary>
		/// Set a particular life style 
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration WithLifestyle(ILifestyle container);

		/// <summary>
		/// Export all attributed types
		/// </summary>
		/// <returns></returns>
		IExportTypeSetConfiguration ExportAttributedTypes();

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate"></param>
		IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition"></param>
		IExportTypeSetConfiguration AndCondition(IExportCondition condition);

		/// <summary>
		/// Exclude a type from being used
		/// </summary>
		/// <param name="exclude"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude);

		/// <summary>
		/// Allows you to filter out types based on the provided where clause
		/// </summary>
		/// <param name="whereClause"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration Select(Func<Type, bool> whereClause);

		/// <summary>
		/// Inspectors will be called for every export strategy that is found
		/// </summary>
		/// <param name="inspector"></param>
		/// <returns></returns>
		IExportTypeSetConfiguration WithInspector(IExportStrategyInspector inspector);
	}
}