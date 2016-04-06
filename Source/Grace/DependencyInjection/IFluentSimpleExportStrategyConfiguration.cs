using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Simple export strategy, used to export a type using only Constructor injection
	/// </summary>
	public interface IFluentSimpleExportStrategyConfiguration
	{
		/// <summary>
		/// Defines the priority to export at
		/// </summary>
		/// <param name="priority">priority for export</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration WithPriority(int priority);

		/// <summary>
		/// Export under a particular key
		/// </summary>
		/// <param name="key">key to associate with export</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration WithKey(object key);

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <param name="exportType">type to export as</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration As(Type exportType);

		/// <summary>
		/// Export as a particular type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IFluentSimpleExportStrategyConfiguration As<T>();

        /// <summary>
        /// Export this type as particular type under the specified key
        /// </summary>
        /// <typeparam name="T">export type</typeparam>
        /// <typeparam name="TKey">type of key</typeparam>
        /// <param name="key">key to export under</param>
        /// <returns>configuration object</returns>
        IFluentSimpleExportStrategyConfiguration AsKeyed<T, TKey>(TKey key);

        /// <summary>
        /// Export this type as particular type under the specified key
        /// </summary>
        /// <param name="exportType">type to export under</param>
        /// <param name="key">export key</param>
        /// <returns>configuration object</returns>
        IFluentSimpleExportStrategyConfiguration AsKeyed(Type exportType, object key);

		/// <summary>
		/// Defines which environment this export should be exported in
		/// </summary>
		/// <param name="environment"></param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration InEnvironment(ExportEnvironment environment);

		/// <summary>
		/// Export this type as a particular name
		/// </summary>
		/// <param name="name"></param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration AsName(string name);
        
		/// <summary>
		/// Mark the export as externally owned, doing so will absolve the container of having to call Dispose when done
		/// </summary>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration ExternallyOwned();

		/// <summary>
		/// Specify a custom Lifestyle container for export.
		/// </summary>
		/// <param name="lifestyle">Lifestyle container for the export</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration UsingLifestyle(ILifestyle lifestyle);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration When(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="conditionDelegate">export condition delegate</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration Unless(ExportConditionDelegate conditionDelegate);

		/// <summary>
		/// Adds a condition to the export
		/// </summary>
		/// <param name="condition">condition for export</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration AndCondition(IExportCondition condition);

		/// <summary>
		/// Adds metadata to an export
		/// </summary>
		/// <param name="metadataName">metadata name</param>
		/// <param name="metadataValue">metadata value</param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration WithMetadata(string metadataName, object metadataValue);

		/// <summary>
		/// Allows you to add custom activation logic to process before the object is returned.
		/// </summary>
		/// <param name="enrichWithDelegate"></param>
		/// <returns>configuration object</returns>
		IFluentSimpleExportStrategyConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate);
	}
}