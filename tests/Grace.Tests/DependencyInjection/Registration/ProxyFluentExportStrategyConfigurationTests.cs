using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.Attributes;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    public class ProxyFluentExportStrategyConfigurationTests
    {
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As([Locate]FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            configuration.As(typeof(IBasicService));

            strategyConfiguration.Received().As(typeof(IBasicService));
        }
    }
}
