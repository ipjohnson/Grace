using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration : IFluentImportPropertyConfiguration
	{
		private ImportPropertyInfo importPropertyInfo;

		private void ProcessCurrentImportProperty()
		{
			if (importPropertyInfo != null)
			{
				exportStrategy.ImportProperty(importPropertyInfo);

				importPropertyInfo = null;
			}
		}

		public IFluentImportPropertyConfiguration ImportProperty(string propertyName)
		{
			ProcessCurrentImportProperty();

			PropertyInfo propertyInfo = exportType.GetRuntimeProperty(propertyName);

			if (propertyInfo == null)
			{
				throw new Exception(string.Format("Could not find property {0} on type {1}", propertyName, exportType.FullName));
			}

			if (!propertyInfo.CanWrite)
			{
				throw new ArgumentException(
					string.Format("Property {0}  is not writable on type {1}", propertyName, exportType.FullName),
					propertyName);
			}

			importPropertyInfo = new ImportPropertyInfo { Property = propertyInfo };

			return this;
		}

		public IFluentImportPropertyConfiguration IsRequired(bool isRequired = true)
		{
			importPropertyInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentImportPropertyConfiguration Consider(ExportStrategyFilter consider)
		{
			importPropertyInfo.ExportStrategyFilter = consider;

			return this;
		}

		public IFluentImportPropertyConfiguration UsingValue(Func<object> valueFunc)
		{
			importPropertyInfo.ValueProvider = new FuncValueProvider<object>(valueFunc);

			return this;
		}

		public IFluentImportPropertyConfiguration UsingValueProvider(IExportValueProvider provider)
		{
			importPropertyInfo.ValueProvider = provider;

			return this;
		}
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		private ImportPropertyInfo importPropertyInfo;

		private void ProcessCurrentImportProperty()
		{
			if (importPropertyInfo != null)
			{
				exportStrategy.ImportProperty(importPropertyInfo);

				importPropertyInfo = null;
			}
		}

		public IFluentImportPropertyConfiguration<T, TProp> ImportProperty<TProp>(Expression<Func<T, TProp>> property)
		{
			MemberExpression member = property.Body as MemberExpression;

			if (member == null)
			{
				throw new ArgumentException("property", "Property must be a property on type" + typeof(T).FullName);
			}

			PropertyInfo propertyInfo =
				member.Member.DeclaringType.GetTypeInfo().GetDeclaredProperty(member.Member.Name);

			ProcessCurrentImportProperty();

			importPropertyInfo = new ImportPropertyInfo { Property = propertyInfo };

			return new FluentImportPropertyConfiguration<T, TProp>(this, importPropertyInfo);
		}
	}

	public class FluentImportPropertyConfiguration<T, TProp> : FluentBaseExportConfiguration<T>, IFluentImportPropertyConfiguration<T, TProp>
	{
		private readonly ImportPropertyInfo importPropertyInfo;

		public FluentImportPropertyConfiguration(IFluentExportStrategyConfiguration<T> strategy,
			ImportPropertyInfo importPropertyInfo) : base(strategy)
		{
			this.importPropertyInfo = importPropertyInfo;
		}

		public IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true)
		{
			importPropertyInfo.IsRequired = isRequired;

			return this;
		}

		public IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider)
		{
			importPropertyInfo.ExportStrategyFilter = consider;

			return this;
		}

		public IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<TProp> valueFunc)
		{
			importPropertyInfo.ValueProvider = new FuncValueProvider<TProp>(valueFunc);

			return this;
		}

		public IFluentImportPropertyConfiguration<T, TProp> UsingValueProvider(IExportValueProvider provider)
		{
			importPropertyInfo.ValueProvider = provider;

			return this;
		}
	}
}
