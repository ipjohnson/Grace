using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ExportInstance
{
    public class ExportInstanceConstantTests
    {
        [Fact]
        public void ExportInstance_Constant_Value()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c =>
            {
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
                c.ExportInstance(basicService).As<IBasicService>();
            });

            var instance1 = container.Locate<IConstructorImportService>();
            var instance2 = container.Locate<IConstructorImportService>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);

            Assert.NotSame(instance1, instance2);
            Assert.Same(basicService, instance1.BasicService);
            Assert.Same(basicService, instance2.BasicService);
        }

        [Fact]
        public void ExportInstance_Constant_Decorated()
        {
            var container = new DependencyInjectionContainer();

            IBasicService basicService = new BasicService { Count = 5 };

            container.Configure(c =>
            {
                c.ExportInstance(basicService);
                c.ExportDecorator(typeof(BasicServiceDecorator)).As(typeof(IBasicService));
            });

            var instance1 = container.Locate<IBasicService>();
            var instance2 = container.Locate<IBasicService>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);

            Assert.NotSame(instance1, instance2);

            Assert.IsType<BasicServiceDecorator>(instance1);
            Assert.IsType<BasicServiceDecorator>(instance2);

            Assert.Equal(5, instance1.Count);
            Assert.Equal(5, instance2.Count);
        }
    }
}
