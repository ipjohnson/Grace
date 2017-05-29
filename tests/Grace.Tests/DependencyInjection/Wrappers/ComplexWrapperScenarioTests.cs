using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class ComplexWrapperScenarioTests
    {
        [Fact]
        public void Func_Wrapped_Enumerable_Dependency_Returns_Correct_Instances()
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

            var enumerableFunc = container.Locate<DependentService<Func<IEnumerable<ISimpleObject>>>>();

            var instances = enumerableFunc.Value().ToArray();

            Assert.NotNull(instances);
            Assert.Equal(5, instances.Length);
            Assert.IsType<SimpleObjectA>(instances[0]);
            Assert.IsType<SimpleObjectB>(instances[1]);
            Assert.IsType<SimpleObjectC>(instances[2]);
            Assert.IsType<SimpleObjectD>(instances[3]);
            Assert.IsType<SimpleObjectE>(instances[4]);
        }

        [Fact]
        public void Func_Wrapped_Enumerable_Meta_Dependency_Returns_Correct_Instances()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Key", 'A');
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Key", 'B');
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Key", 'C');
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Key", 'D');
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Key", 'E');
            });

            var enumerableFunc = container.Locate<DependentService<Func<IEnumerable<Meta<ISimpleObject>>>>>();

            var instances = enumerableFunc.Value().ToArray();

            Assert.NotNull(instances);
            Assert.Equal(5, instances.Length);
            Assert.IsType<SimpleObjectA>(instances[0].Value);
            Assert.Equal('A', instances[0].Metadata["Key"]);

            Assert.IsType<SimpleObjectB>(instances[1].Value);
            Assert.Equal('B', instances[1].Metadata["Key"]);

            Assert.IsType<SimpleObjectC>(instances[2].Value);
            Assert.Equal('C', instances[2].Metadata["Key"]);

            Assert.IsType<SimpleObjectD>(instances[3].Value);
            Assert.Equal('D', instances[3].Metadata["Key"]);

            Assert.IsType<SimpleObjectE>(instances[4].Value);
            Assert.Equal('E', instances[4].Metadata["Key"]);
        }


        [Fact]
        public void Func_Wrapped_Enumerable_Meta_Owned_Dependency_Returns_Correct_Instances()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>().WithMetadata("Key", 'A');
            });

            var enumerableFunc = container.Locate<DependentService<Func<IEnumerable<Meta<Owned<IDisposableService>>>>>>();

            var instances = enumerableFunc.Value().ToArray();

            Assert.NotNull(instances);
            Assert.Equal(1, instances.Length);
            Assert.IsType<DisposableService>(instances[0].Value.Value);
            Assert.Equal('A', instances[0].Metadata["Key"]);

            var disposed = false;

            instances[0].Value.Value.Disposing += (sender, args) => disposed = true;

            instances[0].Value.Dispose();

            Assert.True(disposed);
        }

    }
}
