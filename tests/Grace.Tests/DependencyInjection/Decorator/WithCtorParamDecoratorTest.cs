using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using SimpleFixture.Attributes;
using SimpleFixture.NSubstitute;
using SimpleFixture.xUnit;
using Xunit;

namespace Grace.Tests.DependencyInjection.Decorator
{
    [SubFixtureInitialize]
    public class WithCtorParamDecoratorTest
    {
        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_Named([Freeze] ConstructorParameterInfo constructorParameterInfo,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            configuration.Named("Test");

            Assert.Equal("Test", constructorParameterInfo.ParameterName);
        }

        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_Consider([Freeze] ConstructorParameterInfo constructorParameterInfo,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            ActivationStrategyFilter filter = strategy => true;

            configuration.Consider(filter);

            Assert.Same(filter, constructorParameterInfo.ExportStrategyFilter);
        }

        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_IsDynamic([Freeze] ConstructorParameterInfo constructorParameterInfo,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            configuration.IsDynamic();

            Assert.True(constructorParameterInfo.IsDynamic);
        }

        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_IsRequired([Freeze] ConstructorParameterInfo constructorParameterInfo,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            configuration.IsRequired(false);

            Assert.True(constructorParameterInfo.IsRequired.HasValue);
            Assert.False(constructorParameterInfo.IsRequired.GetValueOrDefault(true));
        }

        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_Use([Freeze] ConstructorParameterInfo constructorParameterInfo,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            constructorParameterInfo.UseType = null;

            configuration.Use(typeof(int));

            Assert.Equal(typeof(int), constructorParameterInfo.UseType);
        }

        [Theory]
        [AutoData]
        public void FluentDecoratorWithCtor_When([Freeze] ConstructorParameterInfo constructorParameterInfo,
            [Freeze]IFluentDecoratorStrategyConfiguration strategyConfiguration,
            FluentDecoratorWithCtorConfiguration<int> configuration)
        {
            configuration.When.InjectedInto<BasicService>();

            strategyConfiguration.Received().When.InjectedInto<BasicService>();
        }
    }
}
