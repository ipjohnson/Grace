using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
		public IFluentExportPropertyConfiguration ExportProperty(string propertyName)
		{
			throw new NotImplementedException();
		}
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		public IFluentExportPropertyConfiguration<T, TProp> ExportProperty<TProp>(Expression<Func<T, TProp>> property)
		{
			MemberExpression member = property.Body as MemberExpression;

			if (member == null)
			{
				throw new ArgumentException("property", "Property must be a property on type" + typeof(T).FullName);
			}

			PropertyInfo propertyInfo =
				member.Member.DeclaringType.GetTypeInfo().GetDeclaredProperty(member.Member.Name);

			ExportPropertyInfo exportPropertyInfo = new ExportPropertyInfo
			                                        {
				                                        PropertyInfo = propertyInfo
			                                        };

			exportStrategy.ExportProperty(exportPropertyInfo);

			return new FluentExportPropertyConfiguration<T, TProp>(exportPropertyInfo, this);
		}
	}

	public class FluentExportPropertyConfiguration : FluentBaseExportConfiguration, IFluentExportPropertyConfiguration
	{
		private readonly ExportPropertyInfo exportPropertyInfo;

		public FluentExportPropertyConfiguration(ExportPropertyInfo exportPropertyInfo,
			IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
			this.exportPropertyInfo = exportPropertyInfo;
		}

		public IFluentExportPropertyConfiguration WithName(string exportName)
		{
			exportPropertyInfo.AddExportName(exportName);

			return this;
		}

		public IFluentExportPropertyConfiguration WithType(Type exportType)
		{
			exportPropertyInfo.AddExportType(exportType);

			return this;
		}

		public IFluentExportPropertyConfiguration WithCondition(IExportCondition exportCondition)
		{
			exportPropertyInfo.ExportCondition = exportCondition;

			return this;
		}
	}

	public class FluentExportPropertyConfiguration<T, TProp> : FluentBaseExportConfiguration<T>,
		IFluentExportPropertyConfiguration<T, TProp>
	{
		private readonly ExportPropertyInfo exportPropertyInfo;

		public FluentExportPropertyConfiguration(ExportPropertyInfo exportPropertyInfo,
			IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.exportPropertyInfo = exportPropertyInfo;
		}

		public IFluentExportPropertyConfiguration<T, TProp> WithName(string exportName)
		{
			exportPropertyInfo.AddExportName(exportName);

			return this;
		}

		public IFluentExportPropertyConfiguration<T, TProp> WithType(Type exportType)
		{
			exportPropertyInfo.AddExportType(exportType);

			return this;
		}

		public IFluentExportPropertyConfiguration<T, TProp> WithCondition(IExportCondition exportCondition)
		{
			exportPropertyInfo.ExportCondition = exportCondition;

			return this;
		}
	}
}