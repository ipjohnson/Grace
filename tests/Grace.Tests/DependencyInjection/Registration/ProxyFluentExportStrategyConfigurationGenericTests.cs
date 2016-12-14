using System;
using System.Linq.Expressions;
using System.Reflection;
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


        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ExternallyOwned([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            configuration.ExternallyOwned();

            strategyConfiguration.Received().ExternallyOwned();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportMethods([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Action<BasicService>> action = service => service.TestMethod();

            configuration.ImportMethod(action);

            strategyConfiguration.Received().ImportMethod(action);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportMembers([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Func<MemberInfo, bool> func = info => true;

            configuration.ImportMembers(func);

            strategyConfiguration.Received().ImportMembers(func);
        }


        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ImportProperty([Locate]FluentWithCtorConfiguration<BasicService, int> configuration,
                                                                      IFluentExportStrategyConfiguration<BasicService> strategyConfiguration)
        {
            Expression<Func<BasicService, int>> func = service => service.Count;

            configuration.ImportProperty(func);

            strategyConfiguration.Received().ImportProperty(func);
        }
    }
}
