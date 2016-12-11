using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.Attributes;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class ProxyFluentExportStrategyConfigurationGenericTests
    {
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As([Locate]FluentWithCtorConfiguration<BasicService, int> configuration, 
                                                              IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.As(typeof(IBasicService));

            strategyConfiguration.Received().As(typeof(IBasicService));
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As_Generic([Locate]FluentWithCtorConfiguration<BasicService, int> configuration, 
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration )
        {
            configuration.As<IBasicService>();

            strategyConfiguration.Received().As<IBasicService>();
        }
        

    }
}
