using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class FluentExportMemberConfigurationTests
    {
        [Theory]
        [AutoData]
        public void FluentExportMemberConfiguration_As(FluentExportMemberConfiguration<BasicService> configuration, 
                                                       IFluentExportStrategyConfiguration<BasicService> fluentConfiguration)
        {
            configuration.As(typeof(IBasicService));

            fluentConfiguration.Received().As(typeof(IBasicService));
        }

        [Theory]
        [AutoData]
        public void FluentExportMemberConfiguration_AsGeneric(FluentExportMemberConfiguration<BasicService> configuration,
                                                       IFluentExportStrategyConfiguration<BasicService> fluentConfiguration)
        {
            configuration.As<IBasicService>();

            fluentConfiguration.Received().As<IBasicService>();
        }
        
        [Theory]
        [AutoData]
        public void FluentExportMemberConfiguration_AsKeyed(FluentExportMemberConfiguration<BasicService> configuration,
                                                            IFluentExportStrategyConfiguration<BasicService> fluentConfiguration)
        {
            configuration.AsKeyed<IBasicService>("Hello");

            fluentConfiguration.Received().AsKeyed<IBasicService>("Hello");
        }


    }
}
