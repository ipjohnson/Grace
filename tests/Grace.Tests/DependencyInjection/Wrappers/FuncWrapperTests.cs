using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class FuncWrapperTests
    {
        [Fact]
        public void Container_Resolve_Func()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var func = container.Locate<Func<IBasicService>>();

            Assert.NotNull(func);

            var basicService = func();

            Assert.NotNull(basicService);
            Assert.IsType<BasicService>(basicService);
        }

        [Fact]
        public void Enumerable_Wrapped_Func_Returns_Correct_Instances()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>();
                c.Export<SimpleObjectB>().As<ISimpleObject>();
                c.Export<SimpleObjectC>().As<ISimpleObject>();
                c.Export<SimpleObjectD>().As<ISimpleObject>();
                c.Export<SimpleObjectE>().As<ISimpleObject>();
            });

            var enumerableFunc = container.Locate<IEnumerable<Func<ISimpleObject>>>();

            var instances = enumerableFunc.Select(f => f()).ToArray();

            Assert.NotNull(instances);
            Assert.Equal(5, instances.Length);
            Assert.IsType<SimpleObjectA>(instances[0]);
            Assert.IsType<SimpleObjectB>(instances[1]);
            Assert.IsType<SimpleObjectC>(instances[2]);
            Assert.IsType<SimpleObjectD>(instances[3]);
            Assert.IsType<SimpleObjectE>(instances[4]);
        }

        [Fact]
        public void FuncWrapper_Null_Test()
        {
            var instance = new FuncWrapperStrategy(null);

            Assert.Null(instance.GetWrappedType(typeof(int)));
        }
    }
}
