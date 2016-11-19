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
    }
}
