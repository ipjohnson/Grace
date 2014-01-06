using Grace.DependencyInjection;

namespace Grace.UnitTests.Classes.Simple
{
	public class ImportOwnedService
	{
		public ImportOwnedService(Owned<IDisposableService> owned)
		{
			Owned = owned;
		}

		public Owned<IDisposableService> Owned { get; private set; }
	}
}