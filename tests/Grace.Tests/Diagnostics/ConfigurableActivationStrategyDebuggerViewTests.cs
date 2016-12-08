using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.Diagnostics
{
    [SubFixtureInitialize]
    public class ConfigurableActivationStrategyDebuggerViewTests
    {
        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_ActivationType(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                              IConfigurableActivationStrategy strategy)
        {
            strategy.ActivationType.Returns(typeof(IBasicService));

            Assert.Equal(typeof(IBasicService), debuggerView.ActivationType);
        }


        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_ExternallyOwned(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                               IConfigurableActivationStrategy strategy)
        {
            strategy.ExternallyOwned.Returns(true);

            Assert.True(debuggerView.ExternallyOwned);
        }
    }
}
