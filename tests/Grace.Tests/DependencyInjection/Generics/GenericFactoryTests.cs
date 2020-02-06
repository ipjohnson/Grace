using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;
using Xunit;

namespace Grace.Tests.DependencyInjection.Generics
{
    public class GenericFactoryTests
    {
        public interface ITestGenericService<T>
        {
            T Value { get; }
        }

        public class Impl1 : ITestGenericService<int>
        {
            public int Value => 10;
        }

        public class Impl2 : ITestGenericService<string>
        {
            public string Value => "Test-String";
        }

        [Fact]
        public void GenericFactory()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddActivationStrategy(new GenericFactoryExportStrategy(container,
                typeof(ITestGenericService<>), FactoryMethod){Lifestyle = new SingletonLifestyle()}));

            var instance = container.Locate<ITestGenericService<int>>();

            Assert.NotNull(instance);
            Assert.IsType<Impl1>(instance);

            Assert.Same(instance, container.Locate<ITestGenericService<int>>());

            var instance2 = container.Locate<ITestGenericService<string>>();

            Assert.NotNull(instance2);
            Assert.IsType<Impl2>(instance2);

            Assert.Same(instance2, container.Locate<ITestGenericService<string>>());

            Assert.ThrowsAny<Exception>(() => container.Locate<ITestGenericService<double>>());


        }

        public class Outer
        {
            public GenericFactoryTests.ITestGenericService<int> IntService { get; }

            public Outer(GenericFactoryTests.ITestGenericService<int> intService)
            {
                IntService = intService;
            }
        }

        [Fact]
        public void GenericFactoryIntoConstructor()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddActivationStrategy(new GenericFactoryExportStrategy(container,
                typeof(ITestGenericService<>), FactoryMethod)));

            var outer = container.Locate<Outer>();
            Assert.NotNull(outer);
            Assert.IsType<Outer>(outer);
            Assert.NotNull(outer.IntService);
            Assert.IsType<GenericFactoryTests.Impl1>(outer.IntService);
        }

        private static object FactoryMethod(Type type)
        {
            if (type == typeof(ITestGenericService<int>))
            {
                return new Impl1();
            }

            if (type == typeof(ITestGenericService<string>))
            {
                return new Impl2();
            }

            throw new Exception("Cannot create");
        }

    }
}
