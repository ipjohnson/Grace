using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Configuration interface for importing a property
	/// </summary>
	public interface IFluentImportPropertyConfiguration : IFluentExportStrategyConfiguration
	{
		/// <summary>
		/// Import the property value after construction. Usually this is done before construction
		/// </summary>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration AfterConstruction();

		/// <summary>
		/// filter to use when importing
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration Consider(ExportStrategyFilter consider);

        /// <summary>
        /// Default value if no other can be found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration DefaultValue(object defaultValue);

		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration IsRequired(bool isRequired = true);

		/// <summary>
		/// Locate with a particular key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration LocateWithKey(object locateKey);

		/// <summary>
		/// Specify a value to use when importing the property
		/// </summary>
		/// <param name="valueFunc">property func</param>
		/// <returns>configuration value</returns>
		IFluentImportPropertyConfiguration UsingValue(Func<object> valueFunc);
        /// <summary>
        /// Specify a value to use when importing the property
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IFluentImportPropertyConfiguration UsingValue(object value);

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
		/// Import the property value after construction. Usually this is done before construction
		/// </summary>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> AfterConstruction();

		/// <summary>
		/// use a filter delegate when importing property
		/// </summary>
		/// <param name="consider">filter delegate</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider);

        /// <summary>
        /// Default value if one can not be found
        /// </summary>
        /// <param name="defaultValue">default value</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration<T, TProp> DefaultValue(TProp defaultValue);

		/// <summary>
		/// Is the property required
		/// </summary>
		/// <param name="isRequired">is required</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true);

		/// <summary>
		/// Locate with a particular key
		/// </summary>
		/// <param name="locateKey">locate key</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> LocateWithKey(object locateKey);

        /// <summary>
        /// Provide value for import
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IFluentImportPropertyConfiguration<T, TProp> UsingValue(TProp value);

        /// <summary>
        /// Provide value for import property
        /// </summary>
        /// <param name="valueFunc">value func</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<TProp> valueFunc);

		/// <summary>
		/// Provide value for import property
		/// </summary>
		/// <param name="valueFunc">value func</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> UsingValue(Func<IInjectionScope,IInjectionContext,TProp> valueFunc);

		/// <summary>
		/// specify value provider for property import
		/// </summary>
		/// <param name="provider">value provider</param>
		/// <returns>configuration object</returns>
		IFluentImportPropertyConfiguration<T, TProp> UsingValueProvider(IExportValueProvider provider);

	}
}