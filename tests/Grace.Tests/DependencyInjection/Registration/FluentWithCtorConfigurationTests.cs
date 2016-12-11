using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class FluentWithCtorConfigurationTests
    {
        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Named_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Named(null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Consider_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Consider(null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_LocateKey_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.LocateWithKey(null));
        }

        [Fact]
        public void FluentWithCtorConfiguration_DefaultValue()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<DependentService<IBasicService>>().WithCtorParam<IBasicService>().DefaultValue(basicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }

        [Fact]
        public void FluentWithCtorConfiguration_DefaultValue_Func()
        {
            var container = new DependencyInjectionContainer();

            var i = 0;

            var basicService = new Func<IBasicService>(() => new BasicService { Count = ++i });

            container.Configure(c => c.Export<DependentService<IBasicService>>().WithCtorParam<IBasicService>().DefaultValue(basicService));

            var instance1 = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance1.Value);
            Assert.Equal(1, instance1.Value.Count);

            var instance2 = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Value);
            Assert.Equal(2, instance2.Value.Count);
        }

        [Fact]
        public void FluentWithCtorConfiguration_LocateKey()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(1);
                c.Export<MultipleService2>().AsKeyed<IMultipleService>(2);
                c.Export<MultipleService3>().AsKeyed<IMultipleService>(3);
                c.Export<MultipleService4>().AsKeyed<IMultipleService>(4);
                c.Export<MultipleService5>().AsKeyed<IMultipleService>(5);
                c.Export<DependentService<IMultipleService>>().As<IDependentService<IMultipleService>>()
                    .WithCtorParam<IMultipleService>().LocateWithKey(3);
            });

            var service = container.Locate<IDependentService<IMultipleService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.IsType<MultipleService3>(service.Value);
        }
    }
}
