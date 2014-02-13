using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
		IFluentWithCtorCollectionConfiguration<TItem> Consider([NotNull]ExportStrategyFilter consider);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortByProperty([NotNull]Func<TItem, IComparable> propertyFunc);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> SortBy([NotNull]IComparer<TItem> comparer);

		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<TItem> Named([NotNull]string name);
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
		IFluentWithCtorCollectionConfiguration<T, TItem> Consider([NotNull]ExportStrategyFilter consider);

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty([NotNull]Func<TItem, IComparable> propertyFunc);

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> SortBy([NotNull]IComparer<TItem> comparer);


		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		IFluentWithCtorCollectionConfiguration<T, TItem> Named([NotNull]string name);
	}

}
