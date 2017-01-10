using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateTwoArgWrapperTests
    {
        public delegate ITwoDependencyService<IBasicService, IMultipleService> ServiceCreator(
            IBasicService basicService, IMultipleService service);

        [Fact]
        public void DelegateTwoArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<TwoDependencyService<IBasicService, IMultipleService>>()
                    .As<ITwoDependencyService<IBasicService, IMultipleService>>();

            });

            var func = container.Locate<ServiceCreator>();

            Assert.NotNull(func);

            var basicService = new BasicService();
            var multipleService = new MultipleService1();

            var instance = func(basicService, multipleService);

            Assert.NotNull(instance);
            Assert.Same(basicService, instance.Dependency1);
            Assert.Same(multipleService, instance.Dependency2);
        }
    }
}
