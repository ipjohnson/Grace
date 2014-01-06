using Grace.DependencyInjection;
using Grace.Logging;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.Logging
{
	public class DebugConsoleLogServiceTests
	{
		/// <summary>
		/// This test is here to prove a simple scenario can be run while the debug console log is used
		/// </summary>
		[Fact]
		public void DebugConsoleLog()
		{
			Logger.SetLogService(new DebugConsoleLogService());

			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.Export<BasicService>().As<IBasicService>());

			IBasicService basicService = container.Locate<IBasicService>();

			Assert.NotNull(basicService);
		}
	}
}