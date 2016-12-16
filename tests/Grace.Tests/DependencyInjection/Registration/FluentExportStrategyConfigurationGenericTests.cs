using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public void FluentExportStrategyConfigurationGeneric_As_Null_Throws(FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.As(null));
        }

        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_ActivationMethod_Null_Throws(FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ActivationMethod(null));
        }
        
        [Theory]
        [AutoData]
        public void FluentExportStrategyConfigurationGeneric_AsKeyed_Null_Key_Throws(FluentExportStrategyConfiguration<BasicService> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.AsKeyed<IBasicService>(null));
        }
    }
}
