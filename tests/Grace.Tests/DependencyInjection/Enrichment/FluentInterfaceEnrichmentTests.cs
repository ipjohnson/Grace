using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enrichment
{
    public class FluentInterfaceEnrichmentTests
    {
        [Fact]
        public void EnrichWithDelegate_Executes()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Apply(b => b.Count = 5));

            var basicService = container.Locate<IBasicService>();

            Assert.NotNull(basicService);
            Assert.Equal(5, basicService.Count);
        }

        [Fact]
        public void ActivationMethod_Executes()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MethodInjectionClass>().ActivationMethod(m => m.InjectMethod(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<MethodInjectionClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.IsType<BasicService>(instance.BasicService);

        }

        [Fact]
        public void Encrichment_Executes()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>().EnrichWithDelegate((scope, context, service) =>
                {
                    service.Count = 5;
                    return service;
                });
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Count);
        }
    }
}
