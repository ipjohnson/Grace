using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class InitializerDecoratorTests
    {
        [Fact]
        public void Initialize_Basic_Service()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInitialize<BasicService>(service => service.Count = 5);
                c.Export<BasicService>().As<IBasicService>();
            });

            var instance = container.Locate<BasicService>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);
        }
    }
}
