using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Factory
{
    public class NoArgFactoryTests
    {
        [Fact]
        public void NoArgFactory_Null_Return_Allowed_Container_Wide()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.AllowInstanceAndFactoryToReturnNull = true);

            container.Configure(c => c.ExportFactory<IBasicService>(() => null));

            Assert.Null(container.Locate<IBasicService>());
        }

        [Fact]
        public void NoArgFactory_Null_Return_Allowed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportFactory<IBasicService>(() => null).AllowNullReturn());

            Assert.Null(container.Locate<IBasicService>());
        }

        [Fact]
        public void NoArgFactory_Null_Return_Throws_Exception()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportFactory<IBasicService>(() => null));

            Assert.Throws<NullValueProvidedException>(() => container.Locate<IBasicService>());
        }

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

        [Fact]
        public void NoArgFactory_Return_Null_Value()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory<IBasicService>(() => null);
                c.Export<DefaultBasicService>().As<IDefaultBasicService>();
            });

            var instance = container.Locate<IDefaultBasicService>();

            Assert.NotNull(instance);
            Assert.Null(instance.BasicService);
        }
    }
}
