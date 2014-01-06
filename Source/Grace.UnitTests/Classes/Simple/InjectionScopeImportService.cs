using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class InjectionScopeImportService
	{
		public InjectionScopeImportService(IInjectionScope injectionScope)
		{
			InjectionScope = injectionScope;
		}

		public IInjectionScope InjectionScope { get; private set; }
	}
}