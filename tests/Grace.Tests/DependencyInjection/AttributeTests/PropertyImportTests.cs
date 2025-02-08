using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class PropertyImportTests
    {
        [Fact]
        public void PropertyImport_Locate_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                                       ExportAttributedTypes().
                                       Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            var instance = container.Locate<IAttributedImportPropertyService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.NotNull(instance.AdaptedBasicService);
        }
    }
}
