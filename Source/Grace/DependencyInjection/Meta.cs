namespace Grace.DependencyInjection
{
	/// <summary>
	/// Meta class is a wrapper around an export 
	/// </summary>
	/// <typeparam name="T">Type to request</typeparam>
	public class Meta<T>
	{
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