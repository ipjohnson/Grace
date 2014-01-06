namespace Grace.DependencyInjection
{
	/// <summary>
	/// Provides disposal scopes for an IInjectionScope
	/// </summary>
	public interface IDisposalScopeProvider
	{
		/// <summary>
		/// Provides a default disposal scope, otherwise the current IInjectionScope will be used
		/// </summary>
		/// <param name="injectionScope"></param>
		/// <returns></returns>
		IDisposalScope ProvideDisposalScope(IInjectionScope injectionScope);
	}
}