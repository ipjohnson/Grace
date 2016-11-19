using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateFourArgWrapperTests
    {
        public delegate IFourDependencyService<SimpleObjectA, SimpleObjectB, SimpleObjectC, SimpleObjectD>
            DependentService(SimpleObjectA a, SimpleObjectB b, SimpleObjectC c, SimpleObjectD d);

        [Fact]
        public void DelegateFourArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(co => co.Export(typeof(FourDependencyService<,,,>)).As(typeof(IFourDependencyService<,,,>)));

            var func = container.Locate<DependentService>();

            Assert.NotNull(func);

            var a = new SimpleObjectA();
            var b = new SimpleObjectB();
            var c = new SimpleObjectC();
            var d = new SimpleObjectD();

            var instance = func(a, b, c, d);

            Assert.NotNull(instance);
            Assert.Same(a, instance.Dependency1);
            Assert.Same(b, instance.Dependency2);
            Assert.Same(c, instance.Dependency3);
            Assert.Same(d, instance.Dependency4);
        }
    }
}
