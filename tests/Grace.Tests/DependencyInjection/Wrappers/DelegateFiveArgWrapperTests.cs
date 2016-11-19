using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateFiveArgWrapperTests
    {
        public delegate IFiveDependencyService<SimpleObjectA, SimpleObjectB, SimpleObjectC, SimpleObjectD, SimpleObjectE>
            DependentService(SimpleObjectA a, SimpleObjectB b, SimpleObjectC c, SimpleObjectD d, SimpleObjectE e);

        [Fact]
        public void DelegateFiveArg_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(co => co.Export(typeof(FiveDependencyService<,,,,>)).As(typeof(IFiveDependencyService<,,,,>)));

            var func = container.Locate<DependentService>();

            Assert.NotNull(func);

            var a = new SimpleObjectA();
            var b = new SimpleObjectB();
            var c = new SimpleObjectC();
            var d = new SimpleObjectD();
            var e = new SimpleObjectE();

            var instance = func(a, b, c, d, e);

            Assert.NotNull(instance);
            Assert.Same(a, instance.Dependency1);
            Assert.Same(b, instance.Dependency2);
            Assert.Same(c, instance.Dependency3);
            Assert.Same(d, instance.Dependency4);
            Assert.Same(e, instance.Dependency5);
        }
    }
}
