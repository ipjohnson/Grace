namespace Grace.DependencyInjection
{
	/// <summary>
	/// Meta class is a wrapper around an export 
	/// </summary>
	/// <typeparam name="T">Type to request</typeparam>
	public class Meta<T>
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="value">exported value</param>
		/// <param name="metadata">metadata associated with export</param>
		public Meta(T value, IExportMetadata metadata)
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
		public IExportMetadata Metadata { get; private set; }
	}
}