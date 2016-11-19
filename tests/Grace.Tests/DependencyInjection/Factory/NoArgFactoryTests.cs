using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class NoArgFactoryTests
    {
        [Fact]
        public void NoArgFactory_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.ExportFactory<IBasicService>(() => basicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Same(basicService, instance.Value);
        }
    }
}
