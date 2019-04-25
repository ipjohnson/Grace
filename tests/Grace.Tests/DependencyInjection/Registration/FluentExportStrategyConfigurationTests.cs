using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class FluentExportStrategyConfigurationTests
    {
        #region null tests
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfiguration_As_Null_Throws(FluentExportStrategyConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.As(null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfiguration_AsKeyed_Null_Type_Throws(FluentExportStrategyConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed(null, 'm'));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfiguration_AsKeyed_Null_Key_Throws(FluentExportStrategyConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed(typeof(IBasicService), null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfiguration_WithMetadata_Null_Key_Throws(FluentExportStrategyConfiguration configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithMetadata(null, 'm'));
        }

        #endregion

        #region Method

        [Fact]
        public void FluentExportStrategyConfiguration_ByInterfaces()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(BasicService)).ByInterfaces());

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void FluentExportStrategyConfiguration_ByInterfaces_Generic()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DependentService<>)).ByInterfaces();
                c.Export<BasicService>().As<IBasicService>();
            });

            var instance = container.Locate<IDependentService<IBasicService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Value);
        }

        [Fact]
        public void FluentExportStrategyConfiguration_ExternallyOwned()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(DisposableService)).ExternallyOwned();
            });

            DisposableService disposableService;
            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                disposableService = scope.Locate<DisposableService>();

                disposableService.Disposing += (sender, args) => disposed = true;
            }

            Assert.False(disposed);
        }

        [Fact]
        public void FluentExportStrategyConfiguration_When_Condition()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(MultipleService1))
                    .As(typeof(IMultipleService))
                    .When.InjectedInto<DependentService<IMultipleService>>();

                c.Export(typeof(MultipleService2))
                    .As(typeof(IMultipleService))
                    .When.InjectedInto<OtherDependentService<IMultipleService>>();
            });

            var instance1 = container.Locate<DependentService<IMultipleService>>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance1.Value);
            Assert.IsType<MultipleService1>(instance1.Value);

            var instance2 = container.Locate<OtherDependentService<IMultipleService>>();

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Value);
            Assert.IsType<MultipleService2>(instance2.Value);
        }


        [Fact]
        public void FluentExportStrategyConfiguration_With_Metadata()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(typeof(BasicService)).As(typeof(IBasicService)).WithMetadata("Data", "Value"));

            var metadata = container.Locate<Meta<IBasicService>>();

            Assert.NotNull(metadata);
            Assert.NotNull(metadata.Value);

            Assert.Equal("Value", metadata.Metadata["Data"]);
        }

        [Fact]
        public void FluentExportStrategyConfiguration_As_Throws_Exception_Open_Closed()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentException>(() => container.Configure(c => c.Export(typeof(DependentService<IBasicService>)).As(typeof(IDependentService<>))));
        }


        [Fact]
        public void FluentExportStrategyConfiguration_AsKeyed_Throws_Exception_Open_Closed()
        {
            var container = new DependencyInjectionContainer();

            Assert.Throws<ArgumentException>(() => container.Configure(c => c.Export(typeof(DependentService<IBasicService>)).AsKeyed(typeof(IDependentService<>), 1)));
        }
        #endregion

        #region ImportAttribute tests

        [Fact]
        public void ImportAttribute_Honored_On_Constructor()
        {
            var container = new DependencyInjectionContainer
            {
                _ =>
                {
                    _.ExportFactory(() => new BasicService {Count = 10}).AsKeyed<IBasicService>("A");
                    _.ExportFactory(() => new BasicService {Count = 5}).AsKeyed<IBasicService>("B");
                }
            };

            var classA = container.Locate<ImportClassA>();

            Assert.NotNull(classA);
            Assert.NotNull(classA.BasicService);
            Assert.Equal(10, classA.BasicService.Count);

            var classB = container.Locate<ImportClassB>();

            Assert.NotNull(classB);
            Assert.NotNull(classB.BasicService);
            Assert.Equal(5, classB.BasicService.Count);
        }

        public class ImportClassA
        {
            public ImportClassA([Import(Key = "A")]IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }


        public class ImportClassB
        {
            public ImportClassB([Import(Key = "B")]IBasicService basicService)
            {
                BasicService = basicService;
            }

            public IBasicService BasicService { get; }
        }

        #endregion
    }
}
