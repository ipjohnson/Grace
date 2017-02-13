using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Grace.Utilities;
using SimpleFixture.NSubstitute;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class FluentWithCtorCollectionConfigurationTests
    {
        [Fact]
        public void FluentWithCtorCollectionConfiguration_SortBy()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => new MultipleService1 { Count = 5 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService2 { Count = 4 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService3 { Count = 3 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService4 { Count = 2 }).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService5 { Count = 1 }).As<IMultipleService>();

                c.Export<DependentService<IEnumerable<IMultipleService>>>()
                    .WithCtorCollectionParam<IEnumerable<IMultipleService>, IMultipleService>()
                    .Named("value")
                    .SortBy(new GenericComparer<IMultipleService>(m => m.Count));
            });

            var instance = container.Locate<DependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);

            var array = instance.Value.ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService5>(array[0]);
            Assert.IsType<MultipleService4>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService2>(array[3]);
            Assert.IsType<MultipleService1>(array[4]);
        }


        [Fact]
        public void FluentWithCtorCollectionConfiguration_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => new MultipleService1 ()).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService2 ()).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService3 ()).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService4 ()).As<IMultipleService>();
                c.ExportFactory(() => new MultipleService5 ()).As<IMultipleService>();

                c.Export<DependentService<IEnumerable<IMultipleService>>>()
                    .WithCtorCollectionParam<IEnumerable<IMultipleService>, IMultipleService>()
                    .Named("value")
                    .Consider(strategy => !strategy.ActivationType.Name.EndsWith("3"));
            });

            var instance = container.Locate<DependentService<IEnumerable<IMultipleService>>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);

            var array = instance.Value.ToArray();

            Assert.Equal(4, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService4>(array[2]);
            Assert.IsType<MultipleService5>(array[3]);
        }
    }
}
