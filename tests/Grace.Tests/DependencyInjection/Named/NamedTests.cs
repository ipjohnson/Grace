using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Named
{
    public class NamedTests
    {
        [Fact]
        public void Export_AsName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().AsName("BasicService"));

            var instance = container.LocateByName("BasicService");

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void Export_TypeSet_ByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<NamedTests>().ByName());

            var instance = container.LocateByName("BasicService");

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }
    }
}
