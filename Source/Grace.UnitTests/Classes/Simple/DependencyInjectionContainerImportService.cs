using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class DependencyInjectionContainerImportService
	{
		public DependencyInjectionContainerImportService(IDependencyInjectionContainer container)
		{
			Container = container;
		}

		public IDependencyInjectionContainer Container { get; private set; }
	}
}