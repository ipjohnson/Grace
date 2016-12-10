using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Conditions;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Conditions
{
    [SubFixtureInitialize]
    public class WhenConditionConfigurationTests
    {
        [Fact]
        public void WhenConditionConfiguration_Null_AddAction_Test()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WhenConditionConfiguration<IConfigurableActivationStrategy>(null, null));
        }


        [Fact]
        public void WhenConditionConfiguration_Null_Strategy_Test()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WhenConditionConfiguration<IConfigurableActivationStrategy>(condition => { }, null));
        }

        [Theory]
        [AutoData]
        public void WhenConditionConfiguration_Null_InjectedInto(WhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.InjectedInto(null));
        }
        
        [Theory]
        [AutoData]
        public void WhenConditionConfiguration_Empty_InjectedInto(WhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration)
        {
            Assert.Throws<ArgumentException>(() => configuration.InjectedInto());
        }

        [Theory]
        [AutoData]
        public void WhenConditionConfiguration_Null_ClassHas(WhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.ClassHas(null));
        }

        [Theory]
        [AutoData]
        public void WhenConditionConfiguration_Null_MeetsCondition_Interface(WhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.MeetsCondition((ICompiledCondition)null));
        }
        
        [Theory]
        [AutoData]
        public void WhenConditionConfiguration_Null_MeetsCondition_Func(WhenConditionConfiguration<IFluentExportStrategyConfiguration> configuration)
        {
            Assert.Throws<ArgumentNullException>(() => configuration.MeetsCondition((Func<IActivationStrategy,StaticInjectionContext,bool>)null));
        }
    }
}
