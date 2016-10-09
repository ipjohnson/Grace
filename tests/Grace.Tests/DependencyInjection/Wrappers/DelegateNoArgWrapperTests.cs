using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class DelegateNoArgWrapperTests
    {
        public delegate IBasicService CreateServiceFunc();

        [Fact]
        public void NoArgDelegate_Create_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var func = container.Locate<CreateServiceFunc>();

            Assert.NotNull(func);

            var instance = func();

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }
    }
}
