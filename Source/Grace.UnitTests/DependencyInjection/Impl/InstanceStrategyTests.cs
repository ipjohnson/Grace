using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class InstanceStrategyTests
	{
		[Fact]
		public void ActivateTest()
		{
			FauxInjectionScope scope = new FauxInjectionScope();
			FauxInjectionContext context = new FauxInjectionContext { RequestingScope = scope };

			BasicService basicService = new BasicService();

			InstanceStrategy<IBasicService> instanceStrategy = new InstanceStrategy<IBasicService>(basicService);

			object activatedObject = instanceStrategy.Activate(scope, context, null);

			Assert.True(ReferenceEquals(activatedObject, basicService));
		}
	}
}