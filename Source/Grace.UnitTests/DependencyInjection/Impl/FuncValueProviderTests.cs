using Grace.DependencyInjection.Impl;
using Grace.UnitTests.Classes.FauxClasses;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Impl
{
	public class FuncValueProviderTests
	{
		[Fact]
		public void ActivateTest()
		{
			FauxInjectionScope scope = new FauxInjectionScope();
			FauxInjectionContext context = new FauxInjectionContext { RequestingScope = scope };

			FuncValueProvider<IBasicService> provider =
				new FuncValueProvider<IBasicService>(() => new BasicService());

			object activated = provider.Activate(scope, context, null, null);

			Assert.NotNull(activated);
			Assert.IsType(typeof(BasicService), activated);
		}
	}
}