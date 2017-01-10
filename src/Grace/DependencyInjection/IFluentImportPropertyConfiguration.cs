namespace Grace.DependencyInjection
{
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
    }
}
