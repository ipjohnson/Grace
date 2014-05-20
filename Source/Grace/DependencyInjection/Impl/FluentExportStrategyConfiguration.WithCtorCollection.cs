using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
		public IFluentWithCtorCollectionConfiguration<TItem> WithCtorParamCollection<TParam, TItem>()
			where TParam : IEnumerable<TItem>
		{
			ConstructorParamInfo paramInfo = new ConstructorParamInfo
			                                 {
				                                 ParameterType = typeof(TParam)
			                                 };

			exportStrategy.WithCtorParam(paramInfo);

			return new FluentWithCtorCollectionConfiguration<TItem>(paramInfo, this);
		}
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		public IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>()
			where TParam : IEnumerable<TItem>
		{
			ConstructorParamInfo paramInfo = new ConstructorParamInfo
			                                 {
				                                 ParameterType = typeof(TParam)
			                                 };

			exportStrategy.WithCtorParam(paramInfo);

			return new FluentWithCtorCollectionConfiguration<T, TItem>(paramInfo, this);
		}
	}

	/// <summary>
	/// Configuration object for constructor parameters
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class FluentWithCtorCollectionConfiguration<TItem> : FluentBaseExportConfiguration,
		IFluentWithCtorCollectionConfiguration<TItem>
	{
		private readonly ConstructorParamInfo paramInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="paramInfo"></param>
		/// <param name="strategy"></param>
		public FluentWithCtorCollectionConfiguration(ConstructorParamInfo paramInfo,
			IFluentExportStrategyConfiguration strategy) : base(strategy)
		{
			this.paramInfo = paramInfo;
		}

		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<TItem> Consider(ExportStrategyFilter consider)
		{
			paramInfo.ExportStrategyFilter = consider;

			return this;
		}

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
		{
			return SortBy(new FuncComparer<TItem>(propertyFunc));
		}

		/// <summary>
		/// Locate with a particular key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<TItem> LocateWithKey(object locateKey)
		{
			paramInfo.LocateKey = locateKey;

			return this;
		}

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<TItem> SortBy(IComparer<TItem> comparer)
		{
			paramInfo.ComparerObject = comparer;

			return this;
		}

		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<TItem> Named(string name)
		{
			paramInfo.ParameterName = name;

			return this;
		}
	}

	/// <summary>
	/// Configuration object for parameters that are collections
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TItem"></typeparam>
	public class FluentWithCtorCollectionConfiguration<T, TItem> : FluentBaseExportConfiguration<T>,
		IFluentWithCtorCollectionConfiguration<T, TItem>
	{
		private readonly ConstructorParamInfo paramInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="paramInfo">param info to configure</param>
		/// <param name="strategy">export strategy</param>
		public FluentWithCtorCollectionConfiguration(ConstructorParamInfo paramInfo,
			IFluentExportStrategyConfiguration<T> strategy) : base(strategy)
		{
			this.paramInfo = paramInfo;
		}

		/// <summary>
		/// Provide a filter for which exports should be used
		/// </summary>
		/// <param name="consider">Filter to use to filter out export strategies</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider)
		{
			paramInfo.ExportStrategyFilter = consider;

			return this;
		}

		/// <summary>
		/// Sort the import collection by a particular property on TItem
		/// </summary>
		/// <param name="propertyFunc">func to use to access property on TItem</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
		{
			return SortBy(new FuncComparer<TItem>(propertyFunc));
		}

		/// <summary>
		/// Locate with a specific key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> LocateWithKey(object locateKey)
		{
			paramInfo.LocateKey = locateKey;

			return this;
		}

		/// <summary>
		/// Sort an import collection before it's being injected
		/// </summary>
		/// <param name="comparer">comparer object to use while sorting</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer)
		{
			paramInfo.ComparerObject = comparer;

			return this;
		}

		/// <summary>
		/// Specify a name of the parameter being configured
		/// </summary>
		/// <param name="name">name of parameter</param>
		/// <returns>configuration object</returns>
		public IFluentWithCtorCollectionConfiguration<T, TItem> Named(string name)
		{
			paramInfo.ParameterName = name;

			return this;
		}
	}
}