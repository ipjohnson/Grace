using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration object for an imoprt collection
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public interface IFluentWithCtorCollectionConfiguration<out TItem> : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> Consider([NotNull] ExportStrategyFilter consider);

		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> Named([NotNull] string name);

		/// <summary>
		/// Locate with a particular key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> LocateWithKey(object locateKey);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortBy([NotNull] IComparer<TItem> comparer);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortByProperty([NotNull] Func<TItem, IComparable> propertyFunc);

	}

	/// <summary>
	/// Configuration object for an imoprt collection
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IFluentWithCtorCollectionConfiguration<T, out TItem> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> Consider([NotNull] ExportStrategyFilter consider);

		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> Named([NotNull] string name);

		/// <summary>
		/// Locate with a specific key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> LocateWithKey(object locateKey);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> SortBy([NotNull] IComparer<TItem> comparer);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty([NotNull] Func<TItem, IComparable> propertyFunc);

	}
}