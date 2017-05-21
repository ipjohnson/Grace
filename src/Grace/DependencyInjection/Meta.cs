namespace Grace.DependencyInjection
{
    /// <summary>
    /// Class that holds a dependency and it's metadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Meta<T>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="value">exported value</param>
        /// <param name="metadata">metadata associated with export</param>
        public Meta(T value, IActivationStrategyMetadata metadata)
        {
            Value = value;
            Metadata = metadata;
        }

        /// <summary>
        /// Resolved Value
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Metadata for the resolved value
        /// </summary>
        public IActivationStrategyMetadata Metadata { get; private set; }
    }

    /// <summary>
    /// Strongly typed metadata class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMetadata"></typeparam>
    public class Meta<T, TMetadata>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="value">exported value</param>
        /// <param name="metadata">metadata associated with export</param>
        public Meta(T value, TMetadata metadata)
        {
            Value = value;
            Metadata = metadata;
        }

        /// <summary>
        /// Resolved Value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Metadata for the resolved value
        /// </summary>
        public TMetadata Metadata { get; }
    }
}
