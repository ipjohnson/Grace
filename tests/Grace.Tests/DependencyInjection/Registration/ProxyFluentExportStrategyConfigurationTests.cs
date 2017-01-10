using System;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
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
        public void ProxyFluentExportStrategyConfiguration_AsKeyed(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            configuration.AsKeyed(typeof(IBasicService),'A');

            strategyConfiguration.Received().AsKeyed(typeof(IBasicService),'A');
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_ByInterfaces(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            Func<Type, bool> func = t => true;

            configuration.ByInterfaces(func);

            strategyConfiguration.Received().ByInterfaces(func);
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

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_UsingLifestyle(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            ICompiledLifestyle lifestyle = new SingletonLifestyle();

            configuration.UsingLifestyle(lifestyle);

            strategyConfiguration.Lifestyle.Received().Custom(lifestyle);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithCtorParam(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            Func<int> func = () => 5;

            configuration.WithCtorParam(func);

            strategyConfiguration.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentExportStrategyConfiguration_WithMetadata(FluentWithCtorConfiguration<int> configuration,
                                                              IFluentExportStrategyConfiguration strategyConfiguration)
        {
            configuration.WithMetadata(5,10);

            strategyConfiguration.Received().WithMetadata(5,10);
        }
    }
}
