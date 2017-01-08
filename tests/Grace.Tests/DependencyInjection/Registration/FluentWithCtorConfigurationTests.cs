using System;
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
        #region Generic
        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Generic_Named_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Named(null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Generic_Consider_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.Consider(null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Generic_LocateKey_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.LocateWithKey(null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Generic_DefaultValue_Func_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.DefaultValue((Func<int>)null));
        }

        [Theory]
        [AutoData]
        public void FluentWithCtorConfiguration_Generic_DefaultValue_Func_Multi_Arg_Null(FluentWithCtorConfiguration<int> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.DefaultValue((Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, int>)null));
        }

        [Fact]
        public void FluentWithCtorConfiguration_Generic_DefaultValue()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<DependentService<IBasicService>>().WithCtorParam<IBasicService>().DefaultValue(basicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }

        [Fact]
        public void FluentWithCtorConfiguration_Generic_DefaultValue_Func()
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
        public void FluentWithCtorConfiguration_Generic_DefaultValue_Func_Multi_Arg()
        {
            var container = new DependencyInjectionContainer();

            var i = 0;

            var basicService = new Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, IBasicService>((scope, staticContext, context) => new BasicService { Count = ++i });

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
        public void FluentWithCtorConfiguration_Generic_Named()
        {
            var container = new DependencyInjectionContainer();

            var basicService = new BasicService();

            container.Configure(c => c.Export<DependentService<IBasicService>>().WithCtorParam<object>().Named("value").DefaultValue(basicService));

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
            Assert.Same(basicService, instance.Value);
        }

        [Fact]
        public void FluentWithCtorConfiguration_Generic_LocateKey()
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

        [Fact]
        public void FluentWithCtorConfiguration_Generic_Consider()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
                c.Export<DependentService<IMultipleService>>().As<IDependentService<IMultipleService>>()
                    .WithCtorParam<IMultipleService>().Consider(s => s.ActivationType.Name.EndsWith("3"));
            });

            var service = container.Locate<IDependentService<IMultipleService>>();

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.IsType<MultipleService3>(service.Value);
        }


        #endregion

        #region WithNamedCtorValue
        [Fact]
        public void WithNamedCtorValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export(typeof(DateTimeImport)).WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }

        [Fact]
        public void WithNamedCtorValueGeneric()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>().WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }


        [Fact]
        public void WithNamedCtorValueGenericNow()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<NowDateTimeImport>().WithNamedCtorValue(() => DateTime.Now));

            NowDateTimeImport import = container.Locate<NowDateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(import.CurrentTime.Date, DateTime.Now.Date);
        }

        [Fact]
        public void ExportNamedValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>());
            container.Configure(c => c.ExportNamedValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }
        #endregion

    }
}
