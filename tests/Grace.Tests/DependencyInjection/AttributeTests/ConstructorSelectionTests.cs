using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class ConstructorSelectionTests
    {
        
        [Fact]
        public void ImportConstructorAttributeTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().As<IMultipleService>();
            });

            var instance = container.Locate<AttributedConstructorService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.MultipleService);
        }
    }
}
