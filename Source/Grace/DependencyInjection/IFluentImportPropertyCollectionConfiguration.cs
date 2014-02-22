using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration interface for exporting a collection
	/// </summary>
	/// <typeparam name="T">exported type</typeparam>
	/// <typeparam name="TItem">property collection type</typeparam>
	public interface IFluentImportPropertyCollectionConfiguration<T, out TItem> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// filter to be used when filling collection
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Sort the collection by property
		/// </summary>
		/// <param name="propertyFunc">function to retrieve property</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

		/// <summary>
		/// Sort the collection using an IComparer
		/// </summary>
		/// <param name="comparer">comparer</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer);
	}
}