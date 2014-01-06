using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class DisposalScopeInjectionService
	{
		public DisposalScopeInjectionService(IDisposalScope disposalScope)
		{
			DisposalScope = disposalScope;
		}

		public IDisposalScope DisposalScope { get; private set; }
	}
}