namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configure import property
    /// </summary>
    public interface IFluentImportPropertyConfiguration : IFluentExportStrategyConfiguration
    {/// <summary>
        /// use a filter delegate when importing property
        /// </summary>
        /// <param name="consider">filter delegate</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration Consider(ActivationStrategyFilter consider);

        /// <summary>
        /// Default value if one can not be found
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
        /// Provide value to be used for property
        /// </summary>
        /// <param name="value">value to use for property</param>
        /// <returns></returns>
        IFluentImportPropertyConfiguration Value(object value);


    }


    /// <summary>
    /// configuration for importing a property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProp"></typeparam>
    public interface IFluentImportPropertyConfiguration<T, in TProp> : IFluentExportStrategyConfiguration<T>
    {
        /// <summary>
        /// use a filter delegate when importing property
        /// </summary>
        /// <param name="consider">filter delegate</param>
        /// <returns>configuration object</returns>
        IFluentImportPropertyConfiguration<T, TProp> Consider(ActivationStrategyFilter consider);

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
        /// Provide value for property
        /// </summary>
        /// <param name="value">property value</param>
        /// <returns></returns>
        IFluentImportPropertyConfiguration<T, TProp> Value(TProp value);
    }
}
