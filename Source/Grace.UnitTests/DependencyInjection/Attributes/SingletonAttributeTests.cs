using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class SingletonAttributeTests
	{
		[Fact]
		public void ProvideLifestyle()
		{
			SingletonAttribute attribute = new SingletonAttribute();

			object Lifestyle = attribute.ProvideLifestyle(typeof(BasicService));

			Assert.NotNull(Lifestyle);
			Assert.IsType(typeof(SingletonLifestyle), Lifestyle);
		}
	}
}