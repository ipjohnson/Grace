using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// This interface configures a property for export
	/// </summary>
	public interface IFluentExportPropertyConfiguration : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Export with a particular name
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration WithName(string exportName);

		/// <summary>
		/// export as a particular type
		/// </summary>
		/// <param name="exportType">type to export as</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration WithType(Type exportType);

		/// <summary>
		/// Export condition for the property
		/// </summary>
		/// <param name="exportCondition">export condition</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration WithCondition(IExportCondition exportCondition);
	}

	/// <summary>
	/// This interface configures a property for export
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProp"></typeparam>
	public interface IFluentExportPropertyConfiguration<T, TProp> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Export with a particular name
		/// </summary>
		/// <param name="exportName">export name</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration<T, TProp> WithName(string exportName);

		/// <summary>
		/// Export with a particular type
		/// </summary>
		/// <param name="exportType">export type</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration<T, TProp> WithType(Type exportType);

		/// <summary>
		/// Export with a particular condition 
		/// </summary>
		/// <param name="exportCondition">export condition</param>
		/// <returns>configuration object</returns>
		IFluentExportPropertyConfiguration<T, TProp> WithCondition(IExportCondition exportCondition);
	}
}