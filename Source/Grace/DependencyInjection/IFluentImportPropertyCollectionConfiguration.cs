using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
	public interface IFluentImportPropertyCollectionConfiguration<T, out TItem> : IFluentExportStrategyConfiguration<T>
	{
		IFluentImportPropertyCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider);

		IFluentImportPropertyCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc);

		IFluentImportPropertyCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer);
	}
}