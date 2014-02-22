namespace Grace.DependencyInjection
{
	public interface IPerRequestScopeProvider
	{
		IInjectionScope ProvideInjectionScope();
	}
}