using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    public class BasicDecoratorTests
    {
        [Fact]
        public void Decorate_BasicService_Returns_Correct_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicServiceDecorator>(instance);
        }

        [Fact]
        public void Decorator_BasicService_With_Second_Type()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
                c.ExportDecorator(typeof(SecondBasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<SecondBasicServiceDecorator>(instance);
        }
    }
}
