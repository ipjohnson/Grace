using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateThreeArgWrapperTests
    {
        public delegate IThreeDependencyService<IBasicService, ISimpleObject, IMultipleService> DependentDelegate(IBasicService basicService, ISimpleObject simpleObject, IMultipleService multipleService);

        [Fact]
        public void ThreeArgDelegate_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(ThreeDependencyService<,,>)).As(typeof(IThreeDependencyService<,,>)));

            var func = container.Locate<DependentDelegate>();

            var basicService = new BasicService();
            var simpleObject = new SimpleObjectA();
            var multipleService = new MultipleService1();

            var instance = func(basicService, simpleObject, multipleService);

            Assert.NotNull(instance);
            Assert.Same(basicService, instance.Dependency1);
            Assert.Same(simpleObject, instance.Dependency2);
            Assert.Same(multipleService, instance.Dependency3);
        }
    }
}
