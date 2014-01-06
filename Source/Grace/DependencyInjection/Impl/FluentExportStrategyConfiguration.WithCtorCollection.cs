using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
		public IFluentWithCtorCollectionConfiguration<TItem> WithCtorParamCollection<TParam, TItem>() where TParam : IEnumerable<TItem>
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
		public IFluentWithCtorCollectionConfiguration<T, TItem> WithCtorCollectionParam<TParam, TItem>() where TParam : IEnumerable<TItem>
		{
			ConstructorParamInfo paramInfo = new ConstructorParamInfo
			{
				ParameterType = typeof(TParam)
			};

			exportStrategy.WithCtorParam(paramInfo);

			return new FluentWithCtorCollectionConfiguration<T, TItem>(paramInfo, this);
		}
	}

	public class FluentWithCtorCollectionConfiguration<TItem> : FluentBaseExportConfiguration, IFluentWithCtorCollectionConfiguration<TItem>
	{
		private readonly ConstructorParamInfo paramInfo;

		public FluentWithCtorCollectionConfiguration(ConstructorParamInfo paramInfo, IFluentExportStrategyConfiguration strategy) : base(strategy)
		{
			this.paramInfo = paramInfo;
		}

		public IFluentWithCtorCollectionConfiguration<TItem> Consider(ExportStrategyFilter consider)
		{
			paramInfo.ExportStrategyFilter = consider;

			return this;
		}

		public IFluentWithCtorCollectionConfiguration<TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
		{
			return SortBy(new FuncComparer<TItem>(propertyFunc));
		}

		public IFluentWithCtorCollectionConfiguration<TItem> SortBy(IComparer<TItem> comparer)
		{
			paramInfo.ComparerObject = comparer;

			return this;
		}
	}

	public class FluentWithCtorCollectionConfiguration<T, TItem> : FluentBaseExportConfiguration<T>, IFluentWithCtorCollectionConfiguration<T, TItem>
	{
		private readonly ConstructorParamInfo paramInfo;

		public FluentWithCtorCollectionConfiguration(ConstructorParamInfo paramInfo,IFluentExportStrategyConfiguration<T> strategy) : base(strategy)
		{
			this.paramInfo = paramInfo;
		}

		public IFluentWithCtorCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider)
		{
			paramInfo.ExportStrategyFilter = consider;

			return this;
		}

		public IFluentWithCtorCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
		{
			return SortBy(new FuncComparer<TItem>(propertyFunc));
		}

		public IFluentWithCtorCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer)
		{
			paramInfo.ComparerObject = comparer;

			return this;
		}
	}
}
