namespace Grace.DependencyInjection
{
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
}
