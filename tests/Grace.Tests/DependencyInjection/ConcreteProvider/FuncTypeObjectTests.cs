using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ConcreteProvider
{
    public class FuncTypeObjectTests
    {
        [Fact]
        public void FuncTypeObject_Resolve()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var func = container.Locate<Func<Type, object>>();

            Assert.NotNull(func);

            var instance = func(typeof(IBasicService));

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }
    }
}
