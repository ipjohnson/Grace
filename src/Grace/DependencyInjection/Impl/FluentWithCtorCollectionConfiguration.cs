using System;
using System.Collections.Generic;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for enumerable properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class FluentWithCtorCollectionConfiguration<T, TItem>  : ProxyFluentExportStrategyConfiguration<T>, IFluentWithCtorCollectionConfiguration<T,TItem>
    {
        private readonly ConstructorParameterInfo _parameterInfo;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="parameterInfo"></param>
        public FluentWithCtorCollectionConfiguration(IFluentExportStrategyConfiguration<T> strategy, ConstructorParameterInfo parameterInfo) : base(strategy)
        {
            _parameterInfo = parameterInfo;
        }

        /// <summary>
        /// Parameter name
        /// </summary>
        public IFluentWithCtorCollectionConfiguration<T, TItem> Named(string name)
        {
            _parameterInfo.ParameterName = name;

            return this;
        }

        /// <summary>
        /// Provide a filter for which exports should be used
        /// </summary>
        /// <param name="consider">Filter to use to filter out export strategies</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorCollectionConfiguration<T, TItem> Consider(ActivationStrategyFilter consider)
        {
            _parameterInfo.ExportStrategyFilter = consider;

            return this;
        }

        /// <summary>
        /// Sort an import collection before it's being injected
        /// </summary>
        /// <param name="comparer">comparer object to use while sorting</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer)
        {
            _parameterInfo.EnumerableComparer = comparer;

            return this;
        }

        /// <summary>
        /// Sort the import collection by a particular property on TItem
        /// </summary>
        /// <param name="propertyFunc">func to use to access property on TItem</param>
        /// <returns>configuration object</returns>
        public IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
        {
            _parameterInfo.EnumerableComparer = new GenericComparer<TItem>(propertyFunc);

            return this;
        }
    }
}
