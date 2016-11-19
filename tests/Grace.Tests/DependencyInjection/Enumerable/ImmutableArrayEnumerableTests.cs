using System.Linq;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class ImmutableArrayEnumerableTests
    {
        [Fact]
        public void Locate_ImmutableArray_Create()
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

            var array = container.Locate<ImmutableArray<IMultipleService>>();

            Assert.NotNull(array);
            
            Assert.NotNull(array);
            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        public class ImportImmutableArray
        {
            public ImportImmutableArray(ImmutableArray<IMultipleService> list)
            {
                List = list;
            }

            public ImmutableArray<IMultipleService> List { get; }
        }

        [Fact]
        public void Import_ImmutableLinkedList()
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

            var instance = container.Locate<ImportImmutableArray>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.List);
            var array = instance.List.ToArray();

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
