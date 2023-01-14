using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;
using Arg = Grace.DependencyInjection.Arg;

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
        
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_One_Arg(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<IMultipleService, IDependentService<IMultipleService>> func = service => null;

            configuration.WithCtorParam(func);

            strategy.Received().ConstructorParameter(NSubstitute.Arg.Is<ConstructorParameterInfo>(info => ReferenceEquals(info.ExportFunc, func)));
        }
        
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_One_Arg_Null(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<IMultipleService, IDependentService<IMultipleService>> func = null;

            Assert.Throws<ArgumentNullException>(() => configuration.WithCtorParam(func));
        }
        
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Two_Arg(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<MultipleService1,MultipleService2, IDependentService<IMultipleService>> func = (service,service2) => null;

            configuration.WithCtorParam(func);

            strategy.Received().ConstructorParameter(NSubstitute.Arg.Is<ConstructorParameterInfo>(info => ReferenceEquals(info.ExportFunc, func)));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Two_Arg_Null(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithCtorParam(null as Func<MultipleService1, MultipleService2, IDependentService<IMultipleService>>));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Three_Arg(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<MultipleService1, MultipleService2, MultipleService3, IDependentService<IMultipleService>> func = (service, service2, service3) => null;

            configuration.WithCtorParam(func);

            strategy.Received().ConstructorParameter(NSubstitute.Arg.Is<ConstructorParameterInfo>(info => ReferenceEquals(info.ExportFunc, func)));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Three_Arg_Null(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithCtorParam(null as Func<MultipleService1, MultipleService2,MultipleService3, IDependentService<IMultipleService>>));
        }


        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Four_Arg(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<MultipleService1, MultipleService2, MultipleService3, MultipleService4, IDependentService<IMultipleService>> func = (service, service2, service3, service4) => null;

            configuration.WithCtorParam(func);

            strategy.Received().ConstructorParameter(NSubstitute.Arg.Is<ConstructorParameterInfo>(info => ReferenceEquals(info.ExportFunc, func)));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Four_Arg_Null(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithCtorParam(null as Func<MultipleService1, MultipleService2, MultipleService3,MultipleService4, IDependentService<IMultipleService>>));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Five_Arg(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Func<MultipleService1, MultipleService2, MultipleService3, MultipleService4,MultipleService5, IDependentService<IMultipleService>> func = (service, service2, service3, service4, service5) => null;

            configuration.WithCtorParam(func);

            strategy.Received().ConstructorParameter(NSubstitute.Arg.Is<ConstructorParameterInfo>(info => ReferenceEquals(info.ExportFunc, func)));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_WithCtorParam_Five_Arg_Null(
            FluentExportStrategyConfiguration<BasicService> configuration, ICompiledExportStrategy strategy)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.WithCtorParam(null as Func<MultipleService1, MultipleService2, MultipleService3, MultipleService4,MultipleService5, IDependentService<IMultipleService>>));
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
        public void FluentExportStrategyConfigurationGeneric_ByInterfaces()
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

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_ImportConstructorMethod()
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

        public class LessConstructor
        {
            public LessConstructor()
            {
                
            }

            public LessConstructor(IBasicService basicService, IConstructorImportService constructorImportService)
            {
                BasicService = basicService;
                ConstructorImportService = constructorImportService;
            }

            public IBasicService BasicService { get; }

            public IConstructorImportService ConstructorImportService { get; }

        }

        [Fact]
        public void FluentExportStrategyConfigurationGeneric_ImportConstructorSelectionMethod()
        {
            var container = new DependencyInjectionContainer(c => c.Behaviors.ConstructorSelection = ConstructorSelectionMethod.LeastParameters);

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<ConstructorImportService>().As<IConstructorImportService>();
                c.Export<MultipleConstructorImport>()
                    .ImportConstructorSelection(ConstructorSelectionMethod.MostParameters);
            });

            var lessConstructor = container.Locate<LessConstructor>();

            Assert.NotNull(lessConstructor);
            Assert.Null(lessConstructor.BasicService);
            Assert.Null(lessConstructor.ConstructorImportService);

            var instance = container.Locate<MultipleConstructorImport>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.BasicService);
            Assert.NotNull(instance.ConstructorImportService);
            Assert.NotNull(instance.ConstructorImportService.BasicService);
        }
    }
}
