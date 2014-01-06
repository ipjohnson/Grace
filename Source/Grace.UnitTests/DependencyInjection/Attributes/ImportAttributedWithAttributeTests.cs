using System.Linq;
using Grace.DependencyInjection;
using Grace.UnitTests.Classes.Attributed;
using Xunit;

namespace Grace.UnitTests.DependencyInjection.Attributes
{
	public class ImportAttributedWithAttributeTests
	{
		[Fact]
		public void ImportMultiple()
		{
			DependencyInjectionContainer container = new DependencyInjectionContainer();

			container.Configure(c => c.ExportAssembly(GetType().Assembly));

			AttributedMultipleSimpleObject multipleSimpleObject =
				container.Locate<AttributedMultipleSimpleObject>();

			Assert.NotNull(multipleSimpleObject);
			Assert.NotNull(multipleSimpleObject.SimpleObjects);
			Assert.Equal(3, multipleSimpleObject.SimpleObjects.Count());
		}
	}
}