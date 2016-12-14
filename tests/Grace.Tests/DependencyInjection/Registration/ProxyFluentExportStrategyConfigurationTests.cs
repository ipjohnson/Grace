using System;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Registration
{
    [SubFixtureInitialize]
    public class ProxyFluentExportStrategyConfigurationTests
    {
        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_As(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            configuration.As(typeof(IBasicService));

            strategyConfiguration.Received().As(typeof(IBasicService));
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ExternallyOwned(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            configuration.ExternallyOwned();

            strategyConfiguration.Received().ExternallyOwned();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportMembers(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            Func<MemberInfo, bool> memberFunc = info => true;

            configuration.ImportMembers(memberFunc);

            strategyConfiguration.Received().ImportMembers(memberFunc);
        }
    }
}
