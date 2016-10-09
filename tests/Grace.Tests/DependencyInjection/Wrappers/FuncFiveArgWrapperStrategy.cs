using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncFiveArgWrapperStrategy
    {
        [Fact]
        public void FuncFiveArg_Locate_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(FiveDependencyService<,,,,>)).As(typeof(IFiveDependencyService<,,,,>)));

            var func = container.Locate<Func<int, double, string, bool, byte, IFiveDependencyService<int, double, string, bool, byte>>>();

            var instance = func(5, 10, "hello", true, 3);

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Dependency1);
            Assert.Equal(10, instance.Dependency2);
            Assert.Equal("hello", instance.Dependency3);
            Assert.Equal(true, instance.Dependency4);
            Assert.Equal(3, instance.Dependency5);
        }
    }
}
