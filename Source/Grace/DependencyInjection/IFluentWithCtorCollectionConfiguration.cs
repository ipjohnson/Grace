using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration object for an imoprt collection
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public interface IFluentWithCtorCollectionConfiguration<TItem> : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortBy(IComparer<TItem> comparer);
	}

	/// <summary>
	/// Configuration object for an imoprt collection
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IFluentWithCtorCollectionConfiguration<T,TItem> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T,TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer);
	}

}
