using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
	public class OwnedTests
	{
		[Fact]
		public void SimpleNonDisposableTest()
		{
			Owned<IBasicService> owned = new Owned<IBasicService>();
			BasicService basicService = new BasicService();

			owned.SetValue(basicService);

			Assert.True(ReferenceEquals(owned.Value, basicService));

			owned.Dispose();
		}

		[Fact]
		public void SimpleDisposableTest()
		{
			Owned<IDisposableService> owned = new Owned<IDisposableService>();
			DisposableService disposableService = new DisposableService();
			bool eventFired = false;

			disposableService.Disposing += (sender, args) => eventFired = true;

			owned.SetValue(disposableService);

			Assert.True(ReferenceEquals(owned.Value, disposableService));

			owned.Dispose();

			Assert.True(eventFired);
		}
	}
}