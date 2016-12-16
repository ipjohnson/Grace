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
    }
}
