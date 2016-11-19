using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncThreeArgWrapperTests
    {
        [Fact]
        public void FuncThreeArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(ThreeDependencyService<,,>)).As(typeof(IThreeDependencyService<,,>)));

            var func = container.Locate<Func<int, double, string, IThreeDependencyService<int, double, string>>>();

            var instance = func(5, 10, "hello");

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
        }
    }
}
