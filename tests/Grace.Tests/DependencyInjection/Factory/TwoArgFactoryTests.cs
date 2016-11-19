using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class TwoArgFactoryTests
    {
        [Fact]
        public void FactoryTwoArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<SimpleObjectA>().As<ISimpleObject>();
                c.ExportFactory<IBasicService, ISimpleObject, ITwoDependencyService<IBasicService, ISimpleObject>>(
                    (basicService, simple) => new TwoDependencyService<IBasicService, ISimpleObject>(basicService, simple));
            });

            var instance = container.Locate<ITwoDependencyService<IBasicService, ISimpleObject>>();

            Assert.NotNull(instance);

            Assert.NotNull(instance.Dependency1);
            Assert.IsType<BasicService>(instance.Dependency1);

            Assert.NotNull(instance.Dependency2);
            Assert.IsType<SimpleObjectA>(instance.Dependency2);
        }
    }
}
