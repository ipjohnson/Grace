using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncTwoArgWrapperTests
    {
        [Fact]
        public void FuncTwoArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(TwoDependencyService<,>)).As(typeof(ITwoDependencyService<,>)));

            var func = container.Locate<Func<int, double, ITwoDependencyService<int, double>>>();

            var instance = func(5, 10);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
        }
    }
}
