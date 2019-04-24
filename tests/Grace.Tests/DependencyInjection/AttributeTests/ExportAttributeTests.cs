using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class ExportAttributeTests
    {
        [Fact]
        public void ExportAttribute_Locate_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                                       ExportAttributedTypes().
                                       Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            var instance = container.Locate<IAttributeBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Metadata_Locate()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                ExportAttributedTypes().
                Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            var instance = container.Locate<Meta<IAttributeBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Equal("Hello", instance.Metadata[123]);
            Assert.Equal("World", instance.Metadata[456]);
        }
    }
}
