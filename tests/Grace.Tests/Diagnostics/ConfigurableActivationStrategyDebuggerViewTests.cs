using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
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

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_HasConditions(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                             IConfigurableActivationStrategy strategy)
        {
            strategy.HasConditions.Returns(true);

            Assert.True(debuggerView.HasConditions);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_Priority(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                             IConfigurableActivationStrategy strategy)
        {
            strategy.Priority.Returns(10);

            Assert.Equal(10, debuggerView.Priority);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_Lifestyle(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                         IConfigurableActivationStrategy strategy,
                                                                         ICompiledLifestyle lifestyle)
        {
            strategy.Lifestyle.Returns(lifestyle);

            Assert.Same(lifestyle, debuggerView.Lifestyle);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_Metadata(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                        IConfigurableActivationStrategy strategy,
                                                                        IActivationStrategyMetadata metadata)
        {
            strategy.Metadata.Returns(metadata);

            Assert.Same(metadata, debuggerView.Metadata);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_Dependencies(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                            IConfigurableActivationStrategy strategy)
        {
            strategy.GetDependencies().Returns(new List<ActivationStrategyDependency> { });

            Assert.False(debuggerView.Dependencies.Any());
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_ExportAs(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                        IConfigurableActivationStrategy strategy)
        {
            strategy.ExportAs.Returns(new[] { typeof(ISimpleObject), typeof(IBasicService) });

            var exports = debuggerView.ExportAs.ToArray();

            Assert.Equal(2, exports.Length);
            Assert.Equal(typeof(IBasicService), exports[0]);
            Assert.Equal(typeof(ISimpleObject), exports[1]);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategyDebuggerView_ExportAsKeyed(ConfigurableActivationStrategyDebuggerView debuggerView,
                                                                        IConfigurableActivationStrategy strategy)
        {
            strategy.ExportAsKeyed.Returns(new[]
            {
                new KeyValuePair<Type, object>(typeof(ISimpleObject), "Hello"),

                new KeyValuePair<Type, object>(typeof(IBasicService), "World")
            });

            var exports = debuggerView.ExportAsKeyed.ToArray();

            Assert.Equal(2, exports.Length);
            Assert.Equal(typeof(IBasicService), exports[0].Key);
            Assert.Equal(typeof(ISimpleObject), exports[1].Key);
        }
    }
}
