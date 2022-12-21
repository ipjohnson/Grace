using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class ImportPropertyTests
    {
        [Fact]
        public void ImportProperty_Generic_Value()
        {
            var service = new BasicService();

            var container = new DependencyInjectionContainer
            {
                _ => _.Export<PropertyInjectionService>().ImportProperty(p => p.BasicService).Value(service)
            };

            var instance = container.Locate<PropertyInjectionService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Same(instance.BasicService, service);
        }


        [Fact]
        public void ImportProperty_Value()
        {
            var service = new BasicService();

            var container = new DependencyInjectionContainer
            {
                _ => _.Export(typeof(PropertyInjectionService)).ImportProperty("BasicService").Value(service)
            };

            var instance = container.Locate<PropertyInjectionService>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Same(instance.BasicService, service);
        }
    }
}
