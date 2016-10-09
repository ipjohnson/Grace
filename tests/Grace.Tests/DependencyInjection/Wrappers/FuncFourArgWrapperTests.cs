using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncFourArgWrapperTests
    {
        [Fact]
        public void FuncFourArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(FourDependencyService<,,,>)).As(typeof(IFourDependencyService<,,,>)));

            var func = container.Locate<Func<int, double, string, bool, IFourDependencyService<int, double, string, bool>>>();

            var instance = func(5, 10, "hello", true);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
            Assert.Equal(true, instance.Dependency4);
        }
    }
}
