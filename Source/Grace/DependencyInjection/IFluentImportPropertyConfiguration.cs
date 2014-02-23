using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration interface for importing a property
	/// </summary>
	public interface IFluentImportPropertyConfiguration : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration IsRequired(bool isRequired = true);

		/// <summary>
		/// filter to use when importing
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Specify a value to use when importing the property
		/// </summary>
		/// <param name="valueFunc">property func</param>
		/// <returns>configuration value</returns>
		IFluentImportPropertyConfiguration UsingValue(Func<object> valueFunc);

		/// <summary>
		/// Value provider for property
		/// </summary>
		/// <param name="provider">value provider</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration UsingValueProvider(IExportValueProvider provider);
	}

	/// <summary>
	/// configuration object for importing a property
	/// </summary>
	/// <typeparam name="T">exporting type</typeparam>
	/// <typeparam name="TProp">type of property to export</typeparam>
	public interface IFluentImportPropertyConfiguration<T, in TProp> : IFluentExportStrategyConfiguration<T>
	{
		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true);

		/// <summary>
		/// use a filter delegate when importing property
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider);

		/// <summary>
		/// Provide value for import property
		/// </summary>
		/// <param name="valueFunc">value func</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<TProp> valueFunc);

		/// <summary>
		/// specify value provider for property import
		/// </summary>
		/// <param name="provider">value provider</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> UsingValueProvider(IExportValueProvider provider);
	}
}