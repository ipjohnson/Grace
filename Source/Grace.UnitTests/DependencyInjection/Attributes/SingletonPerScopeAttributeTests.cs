using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Lifestyle;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class SingletonPerScopeAttributeTests
	{
		[Fact]
		public void ProvideLifestyle()
		{
			SingletonPerScopeAttribute attribute = new SingletonPerScopeAttribute();

			object Lifestyle = attribute.ProvideLifestyle(typeof(BasicService));

			Assert.NotNull(Lifestyle);
			Assert.IsType(typeof(SingletonPerScopeLifestyle), Lifestyle);
		}
	}
}