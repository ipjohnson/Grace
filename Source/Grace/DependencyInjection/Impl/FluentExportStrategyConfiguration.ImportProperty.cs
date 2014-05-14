using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

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

		/// <summary>
		/// Mark a property for import and specify if its required
		/// </summary>
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

		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration IsRequired(bool isRequired = true)
		{
			importPropertyInfo.IsRequired = isRequired;

			return this;
		}

		/// <summary>
		/// filter to use when importing
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration Consider(ExportStrategyFilter consider)
		{
			importPropertyInfo.ExportStrategyFilter = consider;

			return this;
		}

		/// <summary>
		/// Specify a value to use when importing the property
		/// </summary>
		/// <param name="valueFunc">property func</param>
		/// <returns>configuration value</returns>
		public IFluentImportPropertyConfiguration UsingValue(Func<object> valueFunc)
		{
			importPropertyInfo.ValueProvider = new FuncValueProvider<object>(valueFunc);

			return this;
		}

		/// <summary>
		/// Value provider for property
		/// </summary>
		/// <param name="provider">value provider</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration UsingValueProvider(IExportValueProvider provider)
		{
			importPropertyInfo.ValueProvider = provider;

			return this;
		}

		/// <summary>
		/// Import the property value after construction. Usually this is done before construction
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration AfterConstruction()
		{
			importPropertyInfo.AfterConstruction = true;

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

		/// <summary>
		/// Mark a property for Import (using dependency injection container)
		/// </summary>
		/// <returns></returns>
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

	/// <summary>
	/// Configuration object fo importing property
	/// </summary>
	/// <typeparam name="T">type being injected</typeparam>
	/// <typeparam name="TProp">property type import</typeparam>
	public class FluentImportPropertyConfiguration<T, TProp> : FluentBaseExportConfiguration<T>,
		IFluentImportPropertyConfiguration<T, TProp>
	{
		private readonly ImportPropertyInfo importPropertyInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="strategy">strategy</param>
		/// <param name="importPropertyInfo">import info</param>
		public FluentImportPropertyConfiguration(IFluentExportStrategyConfiguration<T> strategy,
			ImportPropertyInfo importPropertyInfo) : base(strategy)
		{
			this.importPropertyInfo = importPropertyInfo;
		}

		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true)
		{
			importPropertyInfo.IsRequired = isRequired;

			return this;
		}

		/// <summary>
		/// use a filter delegate when importing property
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider)
		{
			importPropertyInfo.ExportStrategyFilter = consider;

			return this;
		}

		/// <summary>
		/// Provide value for import property
		/// </summary>
		/// <param name="valueFunc">value func</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<TProp> valueFunc)
		{
			importPropertyInfo.ValueProvider = new FuncValueProvider<TProp>(valueFunc);

			return this;
		}

		/// <summary>
		/// Provide value for import property
		/// </summary>
		/// <param name="valueFunc">value func</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<IInjectionScope, IInjectionContext, TProp> valueFunc)
		{
			importPropertyInfo.ValueProvider = new FuncValueProvider<TProp>(valueFunc);

			return this;
		}

		/// <summary>
		/// specify value provider for property import
		/// </summary>
		/// <param name="provider">value provider</param>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> UsingValueProvider(IExportValueProvider provider)
		{
			importPropertyInfo.ValueProvider = provider;

			return this;
		}

		/// <summary>
		/// Import the property value after construction. Usually this is done before construction
		/// </summary>
		/// <returns>configuration object</returns>
		public IFluentImportPropertyConfiguration<T, TProp> AfterConstruction()
		{
			importPropertyInfo.AfterConstruction = true;

			return this;
		}
	}
}