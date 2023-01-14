using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class ExportRegistrationBlockTests
    {
        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Null(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.Export((Type) null));
        }
        
        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddModule_Null(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddModule(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Enumerable_Null(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.Export((IEnumerable<Type>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Instance(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportInstance((IBasicService)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Instance_Func(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportInstance((Func<IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Instance_FuncScope(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportInstance((Func<IExportLocatorScope, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Instance_FuncScope_StaticContext(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportInstance((Func<IExportLocatorScope,StaticInjectionContext, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Instance_FuncScope_StaticContext_InjectionContext(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportInstance((Func<IExportLocatorScope, StaticInjectionContext,IInjectionContext, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_0_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_1_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IExportLocatorScope, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_2_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IExportLocatorScope, StaticInjectionContext, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_3_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_4_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, IMultipleService, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_Export_Factory_5_Arg(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportFactory((Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext,IMultipleService,ISimpleObject, IBasicService>)null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_ExportWrapper(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportWrapper(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_ExportDecorator(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.ExportDecorator(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddInspector(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddInspector(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddInjectionValueProvider(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddInjectionValueProvider(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddMissing(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddMissingExportStrategyProvider(null));
        }


        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddActivationStrategy(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddActivationStrategy(null));
        }

        [Theory]
        [AutoData]
        public void ExportRegistrationBlock_AddExportStrategyProvider(ExportRegistrationBlock block)
        {
            Assert.Throws<ArgumentNullException>(() => block.AddExportStrategyProvider(null));
        }

        [Fact]
        public void ExportRegistrationBlock_ExportAsKeyed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAsKeyed<BasicService,IBasicService>(5));

            var service = container.Locate<IBasicService>(withKey: 5);

            Assert.NotNull(service);
            Assert.IsType<BasicService>(service);
        }
    }
}
