using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This interface is implemented by classes that can be configured for exporting
	/// </summary>
	public interface IConfigurableExportStrategy : IExportStrategy
	{
		/// <summary>
		/// Lock the export from changing
		/// </summary>
		void Lock();

		/// <summary>
		/// Set the key value for the strategy
		/// </summary>
		/// <param name="key">export key</param>
        IConfigurableExportStrategy SetKey(object key);

		/// <summary>
		/// Add an export name for strategy
		/// </summary>
		/// <param name="exportName">new export name</param>
        IConfigurableExportStrategy AddExportName(string exportName);

		/// <summary>
		/// Add an export type for the strategy
		/// </summary>
		/// <param name="exportType">new type to export</param>
        IConfigurableExportStrategy AddExportType(Type exportType);

        /// <summary>
        /// Add a keyed export type for the strategy
        /// </summary>
        /// <param name="exportType">export type</param>
        /// <param name="key">export key</param>
        IConfigurableExportStrategy AddKeyedExportType(Type exportType, object key);
        
		/// <summary>
		/// Set the priority for the strategy
		/// </summary>
		/// <param name="priority">priority for the export</param>
        IConfigurableExportStrategy SetPriority(int priority);

		/// <summary>
		/// Set the export to be externally owned
		/// </summary>
        IConfigurableExportStrategy SetExternallyOwned();

		/// <summary>
		/// Set the life cycle container for the strategy
		/// </summary>
		/// <param name="container">new Lifestyle container for the export strategy</param>
        IConfigurableExportStrategy SetLifestyleContainer(ILifestyle container);

		/// <summary>
		/// Add a condition to the export
		/// </summary>
		/// <param name="exportCondition">export condition for strategy</param>
        IConfigurableExportStrategy AddCondition(IExportCondition exportCondition);

		/// <summary>
		/// Add metadata to export
		/// </summary>
		/// <param name="name">metadata name</param>
		/// <param name="value">metadata value</param>
        IConfigurableExportStrategy AddMetadata(string name, object value);
	}
}