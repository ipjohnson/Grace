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
    public class FluentExportStrategyConfigurationGenericTests
    {
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_As_Null_Throws(
            FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.As(null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_ActivationMethod_Null_Throws(
            FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ActivationMethod(null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_AsKeyed_Generic_Null_Key_Throws(
            FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed<IBasicService>(null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_AsKeyed_Null_Type_Throws(
            FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed(null, 'C'));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_AsKeyed_Null_Key_Throws(
            FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed(typeof(IBasicService), null));
        }

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_As()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As(typeof(IBasicService)));

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_ByIntefaces()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().ByInterfaces());

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_ByIntefaces_Generic()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().ByInterfaces());

            var instance = container.Locate<IBasicService>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_ImportConstructor()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().ByInterfaces();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
                c.Export<MultipleConstructorImport>()
                    .ImportConstructor(() => new MultipleConstructorImport(Arg.Any<IBasicService>()));
            });

            var instance = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.Null(instance.ConstructorImportService);
        }
    }
}
