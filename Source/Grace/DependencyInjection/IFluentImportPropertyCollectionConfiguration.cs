using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
	public interface IFluentImportPropertyCollectionConfiguration<T, TItem> : IFluentExportStrategyConfiguration<T>
	{
		IFluentImportPropertyCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider);

		IFluentImportPropertyCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

		IFluentImportPropertyCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer);
	}
}
