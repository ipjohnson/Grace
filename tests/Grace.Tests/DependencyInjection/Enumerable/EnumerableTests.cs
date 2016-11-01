using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class EnumerableTests
    {
        [Fact]
        public void Container_LocateEnumerable_ReturnsMultiple()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        [Fact]
        public void Container_ImportEnumerable_ReturnMultiple()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
                c.Export<ImportEnumberableService>().As<IImportEnumberableService>();
            });

            var importService = container.Locate<IImportEnumberableService>();

            Assert.NotNull(importService);
            Assert.NotNull(importService.Enumerable);

            var array = importService.Enumerable.ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        public class ReadOnlyCreator : IEnumerableCreator
        {
            public IEnumerable<T> CreateEnumerable<T>(IExportLocatorScope scope, T[] array)
            {
                return new ReadOnlyCollection<T>(array);
            }
        }

        [Fact]
        public void Container_IEnumerableCreator_ReturnsMultiple()
        {
            var container =
                new DependencyInjectionContainer(c => c.Behaviors.CustomEnumerableCreator = new ReadOnlyCreator());

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });

            var enumerable = container.Locate<IEnumerable<IMultipleService>>();

            Assert.NotNull(enumerable);
            Assert.IsType<ReadOnlyCollection<IMultipleService>>(enumerable);

            var array = enumerable.ToArray();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }
    }
}
