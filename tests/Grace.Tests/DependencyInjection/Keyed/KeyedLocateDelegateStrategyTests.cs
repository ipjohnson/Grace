using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;
using Grace.DependencyInjection.Impl.KnownTypeStrategies;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class KeyedLocateDelegateStrategyTests
    {

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void KeyedLocateDelegateStrategy_GetActivationExpression_Throws(KeyedLocateDelegateStrategy strategy)
        {
            Assert.Throws<NotSupportedException>(() => strategy.GetDecoratorActivationExpression(null, null, null));

        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void KeyedLocateDelegateStrategy_SecondaryStrategy(KeyedLocateDelegateStrategy strategy)
        {
            Assert.Throws<NotSupportedException>(() => strategy.AddSecondaryStrategy(null));

            var array = strategy.SecondaryStrategies().ToArray();
        }
    }
}
