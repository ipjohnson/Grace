using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Wrappers
{
    public class LazyMetadataWrapperTests
    {
        [Fact]
        public void LazyMetadata_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("Name", 1);
                c.Export<MultipleService2>().As<IMultipleService>().WithMetadata("Name", 2);
                c.Export<MultipleService3>().As<IMultipleService>().WithMetadata("Name", 3);
                c.Export<MultipleService4>().As<IMultipleService>().WithMetadata("Name", 4);
                c.Export<MultipleService5>().As<IMultipleService>().WithMetadata("Name", 5);
            });

            var array = container.Locate<Lazy<IMultipleService, IActivationStrategyMetadata>[]>();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);

            Assert.Equal(1, array[0].Metadata["Name"]);
            Assert.Equal(2, array[1].Metadata["Name"]);
            Assert.Equal(3, array[2].Metadata["Name"]);
            Assert.Equal(4, array[3].Metadata["Name"]);
            Assert.Equal(5, array[4].Metadata["Name"]);

            Assert.IsType<MultipleService1>(array[0].Value);
            Assert.IsType<MultipleService2>(array[1].Value);
            Assert.IsType<MultipleService3>(array[2].Value);
            Assert.IsType<MultipleService4>(array[3].Value);
            Assert.IsType<MultipleService5>(array[4].Value);
        }

        public class MetadataValue
        {
            public int IntValue { get; set; }
        }

        [Fact]
        public void LazyMetadata_StronglyTyped()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("IntValue", 1);
                c.Export<MultipleService2>().As<IMultipleService>().WithMetadata("IntValue", 2);
                c.Export<MultipleService3>().As<IMultipleService>().WithMetadata("IntValue", 3);
                c.Export<MultipleService4>().As<IMultipleService>().WithMetadata("IntValue", 4);
                c.Export<MultipleService5>().As<IMultipleService>().WithMetadata("IntValue", 5);
            });

            var array = container.Locate<Lazy<IMultipleService, MetadataValue>[]>();

            Assert.NotNull(array);
            Assert.Equal(5, array.Length);

            Assert.Equal(1, array[0].Metadata.IntValue);
            Assert.Equal(2, array[1].Metadata.IntValue);
            Assert.Equal(3, array[2].Metadata.IntValue);
            Assert.Equal(4, array[3].Metadata.IntValue);
            Assert.Equal(5, array[4].Metadata.IntValue);

            Assert.IsType<MultipleService1>(array[0].Value);
            Assert.IsType<MultipleService2>(array[1].Value);
            Assert.IsType<MultipleService3>(array[2].Value);
            Assert.IsType<MultipleService4>(array[3].Value);
            Assert.IsType<MultipleService5>(array[4].Value);
        }

        public class StringMetadataValue
        {
            public string StringValue { get; set; }
        }
        
        [Fact]
        public void LazyMetadata_StronglyTyped_ConvertValue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("StringValue", 1);
            });

            var lazy = container.Locate<Lazy<IMultipleService, StringMetadataValue>>();

            Assert.Equal("1", lazy.Metadata.StringValue);
            Assert.IsType<MultipleService1>(lazy.Value);
        }

        public class ReadOnlyMetadata
        {
            public ReadOnlyMetadata(int intValue)
            {
                IntValue = intValue;
            }

            public int IntValue { get; }
        }

        [Fact]
        public void LazyMetadata_StronglyTyped_ReadonlyMetadata()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<MultipleService1>().As<IMultipleService>().WithMetadata("IntValue", 10));

            var instance = container.Locate<Lazy<IMultipleService, ReadOnlyMetadata>>();

            Assert.NotNull(instance);
            Assert.IsType<MultipleService1>(instance.Value);
            Assert.Equal(10, instance.Metadata.IntValue);
        }
    }
}
