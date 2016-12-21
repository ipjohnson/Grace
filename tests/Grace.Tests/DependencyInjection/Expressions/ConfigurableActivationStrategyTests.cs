using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.Tests.Classes.Simple;
using SimpleFixture.Attributes;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Expressions
{
    [SubFixtureInitialize]
    public class ConfigurableActivationStrategyTests
    {
        public class FauxConfigurableActivationStrategy : ConfigurableActivationStrategy
        {
            public FauxConfigurableActivationStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
            {
            }

            public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.FrameworkExportStrategy;
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategy_Set_Metadata_Twice([Locate]FauxConfigurableActivationStrategy strategy)
        {
            strategy.SetMetadata("Hello", "World");

            Assert.Equal("World", strategy.Metadata["Hello"]);

            strategy.SetMetadata("Hello", "Goodbye");

            Assert.Equal("Goodbye", strategy.Metadata["Hello"]);
        }

        [Theory]
        [AutoData]
        public void ConfigurableActivationStrategy_GetDependency_Returns_Empty([Locate]FauxConfigurableActivationStrategy strategy)
        {
            Assert.Empty(strategy.GetDependencies(null));
        }

        [Fact]
        public void ConfigurableActivationStrategy_DebuggerString()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
                c.Export<MultipleService1>().AsKeyed<IMultipleService>('A').AsKeyed<IMultipleService>('B');
                c.Export<MultipleService2>();
                c.Export<MultipleService3>().As<MultipleService3>().As<IMultipleService>();
            });

            var displayStringProp = typeof(ConfigurableActivationStrategy).GetProperty("DebuggerDisplayString",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var displayNameProp = typeof(ConfigurableActivationStrategy).GetProperty("DebuggerNameDisplayString",
                BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var strategy in container.StrategyCollectionContainer.GetAllStrategies())
            {
                Assert.True(!string.IsNullOrEmpty(displayStringProp.GetValue(strategy) as string));
                Assert.True(!string.IsNullOrEmpty(displayNameProp.GetValue(strategy) as string));
            }
        }
    }
}
