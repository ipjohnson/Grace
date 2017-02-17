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

namespace Grace.Tests.DependencyInjection.Decorator
{
    [SubFixtureInitialize]
    public class ProxyFluentDecoratorStrategyConfigurationTests
    {
        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_As(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            configuration.As(typeof(int));

            strategy.Received().As(typeof(int));
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_When(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            configuration.When.InjectedInto<BasicService>();

            strategy.Received().When.InjectedInto<BasicService>();
        }


        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_ApplyAfterLifestyle(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            configuration.ApplyAfterLifestyle();

            strategy.Received().ApplyAfterLifestyle();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            configuration.WithCtorParam<IBasicService>();

            strategy.Received().WithCtorParam<IBasicService>();
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam_One(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            Func<int, IBasicService> func = x => new BasicService();

            configuration.WithCtorParam(func);

            strategy.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam_Two(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            Func<int, string, IBasicService> func = (i, s) => new BasicService();

            configuration.WithCtorParam(func);

            strategy.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam_Three(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            Func<int, string, double, IBasicService> func = (i, s, d) => new BasicService();

            configuration.WithCtorParam(func);

            strategy.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam_Four(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            Func<int, string, double, float, IBasicService> func = (i, s, d, f) => new BasicService();

            configuration.WithCtorParam(func);

            strategy.Received().WithCtorParam(func);
        }

        [Theory]
        [AutoData]
        public void ProxyFluentDecoratorStrategyConfiguration_WithCtorParam_Five(IFluentDecoratorStrategyConfiguration strategy, ProxyFluentDecoratorStrategyConfiguration configuration)
        {
            Func<int, string, double, float,byte, IBasicService> func = (i, s, d, f, b) => new BasicService();

            configuration.WithCtorParam(func);

            strategy.Received().WithCtorParam(func);
        }
    }
}
