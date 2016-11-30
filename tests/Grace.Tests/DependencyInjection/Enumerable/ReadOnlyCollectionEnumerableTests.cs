using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class ReadOnlyCollectionEnumerableTests
    {
        [Fact]
        public void Locate_ReadOnlyCollection_Enumerable()
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

            var enumerable = container.Locate<ReadOnlyCollection<IMultipleService>>();

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

        public class ImportReadOnlyCollectionClass
        {
            public ImportReadOnlyCollectionClass(ReadOnlyCollection<IMultipleService> list)
            {
                List = list;
            }

            public ReadOnlyCollection<IMultipleService> List { get; }
        }

        [Fact]
        public void Import_List_Enumerable()
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


            var instance = container.Locate<ImportReadOnlyCollectionClass>();

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

        [Fact]
        public void Import_Sorted_ReadOnlyCollection()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => new MultipleService5 { Count = 5 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService4 { Count = 4 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService3 { Count = 3 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService2 { Count = 2 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService1 { Count = 1 }).As<IMultipleService>();
                c.Export<ImportReadOnlyCollectionClass>()
                    .WithCtorCollectionParam<IReadOnlyCollection<IMultipleService>, IMultipleService>()
                    .SortByProperty(m => m.Count);
            });

            var instance = container.Locate<ImportReadOnlyCollectionClass>();

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
