using System;
using System.Linq;
using Grace.DependencyInjection;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;
using Grace.DependencyInjection.Impl.EnumerableStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.Tests.DependencyInjection.Enumerable
{
    public class BaseGenericEnumerableStrategyTests
    {
        public class LocalBaseGenericEnumerableStrategy : BaseGenericEnumerableStrategy
        {
            public LocalBaseGenericEnumerableStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
            {

            }

            public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
            {
                throw new NotImplementedException();
            }

            public override IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
            {
                throw new NotImplementedException();
            }
        }

        [Theory]
        [AutoData]
        [SubFixtureInitialize]
        public void BaseGenericEnumerableStrategy_SecondaryStrategy(LocalBaseGenericEnumerableStrategy strategy, ICompiledExportStrategy addStrategy)
        {
            strategy.AddSecondaryStrategy(addStrategy);

            var strategies = strategy.SecondaryStrategies().ToArray();

            Assert.Equal(1, strategies.Length);
            Assert.Same(addStrategy, strategies[0]);
        }
    }
}
