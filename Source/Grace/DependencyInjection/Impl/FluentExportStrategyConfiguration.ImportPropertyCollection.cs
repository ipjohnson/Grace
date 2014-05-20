using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		public IFluentImportPropertyCollectionConfiguration<T, TItem> ImportCollectionProperty<TItem>(
			Expression<Func<T, IEnumerable<TItem>>> property)
		{
			MemberExpression member = property.Body as MemberExpression;

			if (member == null)
			{
				throw new ArgumentException("property", "Property must be a property on type" + typeof(T).FullName);
			}

			PropertyInfo propertyInfo =
				member.Member.DeclaringType.GetTypeInfo().GetDeclaredProperty(member.Member.Name);

			ImportPropertyInfo newImportPropertyInfo = new ImportPropertyInfo
			                                           {
				                                           Property = propertyInfo
			                                           };

			exportStrategy.ImportProperty(newImportPropertyInfo);

			return new FluentImportPropertyCollectionConfiguration<T, TItem>(newImportPropertyInfo, this);
		}
	}

	public class FluentImportPropertyCollectionConfiguration<T, TItem> : FluentBaseExportConfiguration<T>,
		IFluentImportPropertyCollectionConfiguration<T, TItem>
	{
		private readonly ImportPropertyInfo newImportPropertyInfo;

		public FluentImportPropertyCollectionConfiguration(ImportPropertyInfo newImportPropertyInfo,
			IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.newImportPropertyInfo = newImportPropertyInfo;
		}

		public IFluentImportPropertyCollectionConfiguration<T, TItem> Consider(ExportStrategyFilter consider)
		{
			newImportPropertyInfo.ExportStrategyFilter = consider;

			return this;
		}

		public IFluentImportPropertyCollectionConfiguration<T, TItem> LocateWithKey(object locateKey)
		{
			newImportPropertyInfo.LocateKey = locateKey;

			return this;
		}

		public IFluentImportPropertyCollectionConfiguration<T, TItem> SortByProperty(Func<TItem, IComparable> propertyFunc)
		{
			newImportPropertyInfo.ComparerObject = new FuncComparer<TItem>(propertyFunc);

			return this;
		}

		public IFluentImportPropertyCollectionConfiguration<T, TItem> SortBy(IComparer<TItem> comparer)
		{
			newImportPropertyInfo.ComparerObject = comparer;

			return this;
		}
	}
}