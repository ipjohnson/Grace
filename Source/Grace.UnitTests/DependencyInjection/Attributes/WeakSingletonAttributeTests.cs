using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class WeakSingletonAttributeTests
	{
		[Fact]
		public void ProvideLifestyle()
		{
			WeakSingletonAttribute attribute = new WeakSingletonAttribute();

			object container = attribute.ProvideLifestyle(typeof(BasicService));

			Assert.NotNull(container);
			Assert.IsType(typeof(WeakSingletonLifestyle), container);
		}
	}
}