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
		/// <param name="locateKey"></param>
		/// <returns>list of exports</returns>
		List<T> ActivateAll<T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey);

		/// <summary>
		/// Activate all instances of type as Meta(T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TLazy"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		List<TLazy> ActivateAllLazy<TLazy, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TLazy : Lazy<T>;

		/// <summary>
		/// Activate all instances of type as Meta(T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TOwned"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		List<TOwned> ActivateAllOwned<TOwned, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TOwned : Owned<T> where T : class;

		/// <summary>
		/// Activate all instances of type as Meta(T)
		/// </summary>
		/// <typeparam name="TMeta"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="injectionContext"></param>
		/// <param name="filter"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		List<TMeta> ActivateAllMeta<TMeta, T>(IInjectionContext injectionContext, ExportStrategyFilter filter, object locateKey)
			where TMeta : Meta<T>;

		/// <summary>
		/// Activate the first export strategy that meets conditions
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <param name="exportType">export type</param>
		/// <param name="injectionContext">injection context</param>
		/// <param name="filter">export filter</param>
		/// <param name="locateKey"></param>
		/// <returns>export object</returns>
		object Activate(string exportName,
							 Type exportType,
							 IInjectionContext injectionContext,
							 ExportStrategyFilter filter, 
							 object locateKey);

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