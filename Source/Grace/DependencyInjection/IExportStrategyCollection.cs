using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Represents a collection of export strategies for a particular export name
	/// </summary>
	public interface IExportStrategyCollection
	{
		/// <summary>
		/// A enumerable of export strategies
		/// </summary>
		IEnumerable<IExportStrategy> ExportStrategies { get; }

		/// <summary>
		/// Activate all instances of a type
		/// </summary>
		/// <typeparam name="T">type to locate</typeparam>
		/// <param name="injectionContext">injection context</param>
		/// <param name="filter">export filter</param>
		/// <returns>list of exports</returns>
		List<T> ActivateAll<T>(IInjectionContext injectionContext, ExportStrategyFilter filter);

		/// <summary>
		/// Activate the first export strategy that meets conditions
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <param name="exportType">export type</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="filter">export filter</param>
		/// <returns>export object</returns>
		object Activate(string exportName,
			Type exportType,
			IInjectionContext injectionContext,
			ExportStrategyFilter filter);

		/// <summary>
		/// Add an export strategy to the collection
		/// </summary>
		/// <param name="exportStrategy">new export strategy</param>
		void AddExport(IExportStrategy exportStrategy);

		/// <summary>
		/// Remove an export strategy from the collection
		/// </summary>
		/// <param name="exportStrategy">old export strategy</param>
		void RemoveExport(IExportStrategy exportStrategy);
	}
}