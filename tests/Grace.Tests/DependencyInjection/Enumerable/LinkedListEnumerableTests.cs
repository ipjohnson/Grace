using System;
using System.Linq;
using Grace.Data.Immutable;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.EnumerableStrategies;
using Grace.Tests.Classes.Simple;
using SimpleFixture.Attributes;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class LinkedListEnumerableTests
    {
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void ImmutableLinkedList_Decorator_Throws_Exception([Locate]ImmutableLinkListStrategy strategy)
        {
            Assert.Throws<NotSupportedException>(() => strategy.GetDecoratorActivationExpression(null, null, null));
        }

        [Fact]
        public void Locate_ImmutableLinkedList()
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

            var enumerable = container.Locate<ImmutableLinkedList<IMultipleService>>();

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

        public class ImportImmutableLinkedList
        {
            public ImportImmutableLinkedList(ImmutableLinkedList<IMultipleService> list)
            {
                List = list;
            }

            public ImmutableLinkedList<IMultipleService> List { get; }
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

            var instance = container.Locate<ImportImmutableLinkedList>();

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
