using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grace.DependencyInjection;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;
using Grace.DependencyInjection.Impl.EnumerableStrategies;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class BaseGenericEnumerableStrategyTests
    {
        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void BaseGenericEnumerableStrategy_SecondaryStrategy(BaseGenericEnumerableStrategy strategy, ICompiledExportStrategy addStrategy)
        {
            strategy.AddSecondaryStrategy(strategy);

            var strategies = strategy.SecondaryStrategies().ToArray();

            Assert.Equal(1, strategies.Length);
            Assert.Same(addStrategy, strategies[0]);
        }
    }
}
