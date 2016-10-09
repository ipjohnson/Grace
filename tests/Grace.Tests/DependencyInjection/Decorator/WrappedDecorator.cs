using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class WrappedDecorator
    {
        [Fact]
        public void FuncWrapped_BasicService_Decorator()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceFuncDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceFuncDecorator>(instance);

            instance.TestMethod();
        }

        [Fact]
        public void LazyWrapped_BasicService_Decorator()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(LazyBasicService)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<LazyBasicService>(instance);

            instance.TestMethod();
        }

        [Fact]
        public void LazyFunc_Decorator_BasicService()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceLazyFuncDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceLazyFuncDecorator>(instance);

            instance.TestMethod();
        }
    }
}
