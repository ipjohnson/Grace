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
    }
}
