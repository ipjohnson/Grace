using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{

    /// <summary>
    /// Configuration object for an imoprt collection
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IFluentWithCtorCollectionConfiguration<T, out TItem> : IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        IFluentWithCtorCollectionConfiguration<T, TItem> Named(string name);

        /// <summary>
        /// Provide a filter for which exports should be used
        /// </summary>
        /// <param name="consider">Filter to use to filter out export strategies</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorCollectionConfiguration<T, TItem> Consider(ActivationStrategyFilter consider);

        /// <summary>
        /// Sort an import collection before it's being injected
        /// </summary>
        /// <param name="comparer">comparer object to use while sorting</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer);

        /// <summary>
        /// Sort the import collection by a particular property on TItem
        /// </summary>
        /// <param name="propertyFunc">func to use to access property on TItem</param>
        /// <returns>configuration object</returns>
        IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

    }
}
