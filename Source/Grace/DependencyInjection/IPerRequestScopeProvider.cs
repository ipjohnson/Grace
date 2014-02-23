namespace Grace.DependencyInjection
{
	/// <summary>
	/// This interface is used by per request singleton container
	/// </summary>
	public interface IPerRequestScopeProvider
	{
		/// <summary>
		/// Provide a scope
		/// </summary>
		/// <returns>returns a scope</returns>
		IInjectionScope ProvideInjectionScope();
	}
}