using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
	public partial class FluentExportStrategyConfiguration
	{
		/// <summary>
		/// Export a property by name
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IFluentExportPropertyConfiguration ExportProperty(string propertyName)
		{
			PropertyInfo propertyInfo =
				exportType.GetTypeInfo().GetDeclaredProperty(propertyName);

			ExportPropertyInfo exportPropertyInfo = new ExportPropertyInfo
			{
				PropertyInfo = propertyInfo
			};

			exportStrategy.ExportProperty(exportPropertyInfo);

			return new FluentExportPropertyConfiguration(exportPropertyInfo, this);
		}
	}

	public partial class FluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Exports  a property on an object
		/// </summary>
		/// <typeparam name="TProp"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
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


	/// <summary>
	/// Configuration object for exporting a property
	/// </summary>
	public class FluentExportPropertyConfiguration : FluentBaseExportConfiguration, IFluentExportPropertyConfiguration
	{
		private readonly ExportPropertyInfo exportPropertyInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportPropertyInfo">property info to export</param>
		/// <param name="strategy">export strategy</param>
		public FluentExportPropertyConfiguration(ExportPropertyInfo exportPropertyInfo,
			IFluentExportStrategyConfiguration strategy)
			: base(strategy)
		{
			this.exportPropertyInfo = exportPropertyInfo;
		}

		/// <summary>
		/// Export with a particular name
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration WithName(string exportName)
		{
			exportPropertyInfo.AddExportName(exportName);

			return this;
		}

		/// <summary>
		/// export as a particular type
		/// </summary>
		/// <param name="exportType">type to export as</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration WithType(Type exportType)
		{
			exportPropertyInfo.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export condition for the property
		/// </summary>
		/// <param name="exportCondition">export condition</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration WithCondition(IExportCondition exportCondition)
		{
			exportPropertyInfo.ExportCondition = exportCondition;

			return this;
		}
	}

	/// <summary>
	/// Conifugration object for exporting a property
	/// </summary>
	/// <typeparam name="T">type being exported</typeparam>
	/// <typeparam name="TProp">property type being exported</typeparam>
	public class FluentExportPropertyConfiguration<T, TProp> : FluentBaseExportConfiguration<T>,
		IFluentExportPropertyConfiguration<T, TProp>
	{
		private readonly ExportPropertyInfo exportPropertyInfo;

		/// <summary>
		/// Deafult constructor
		/// </summary>
		/// <param name="exportPropertyInfo"></param>
		/// <param name="strategy"></param>
		public FluentExportPropertyConfiguration(ExportPropertyInfo exportPropertyInfo,
			IFluentExportStrategyConfiguration<T> strategy)
			: base(strategy)
		{
			this.exportPropertyInfo = exportPropertyInfo;
		}

		/// <summary>
		/// Export with a particular name
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration<T, TProp> WithName(string exportName)
		{
			exportPropertyInfo.AddExportName(exportName);

			return this;
		}

		/// <summary>
		/// Export with a particular type
		/// </summary>
		/// <param name="exportType">export type</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration<T, TProp> WithType(Type exportType)
		{
			exportPropertyInfo.AddExportType(exportType);

			return this;
		}

		/// <summary>
		/// Export with a particular condition 
		/// </summary>
		/// <param name="exportCondition">export condition</param>
		/// <returns>configuration object</returns>
		public IFluentExportPropertyConfiguration<T, TProp> WithCondition(IExportCondition exportCondition)
		{
			exportPropertyInfo.ExportCondition = exportCondition;

			return this;
		}
	}
}