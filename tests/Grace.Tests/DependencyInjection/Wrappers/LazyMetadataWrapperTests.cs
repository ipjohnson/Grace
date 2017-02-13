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
    }
}
