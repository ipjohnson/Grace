using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Optional;
using Xunit;

namespace Grace.Tests.ThridParty.Optional
{
    /// <summary>
    /// OPtional strategy provider
    /// </summary>
    public class OptionalStrategyProvider : IMissingExportStrategyProvider
    {
        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.ActivationType.IsConstructedGenericType &&
                request.ActivationType.GetGenericTypeDefinition() == typeof(Option<>))
            {
                yield return new OptionalStrategy(scope);
            }
        }
    }

    public class OptionalTests
    {
        [Fact]
        public void OptionalIntHasValueFalse()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddMissingExportStrategyProvider(new OptionalStrategyProvider()));

            var instance = container.Locate<Option<int>>();

            Assert.False(instance.HasValue);
        }

        [Fact]
        public void OptionalIntHasValueTrue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(5);
                c.AddMissingExportStrategyProvider(new OptionalStrategyProvider());
            });

            var instance = container.Locate<Option<int>>();

            Assert.True(instance.HasValue);
            Assert.Equal(5, instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceHasValueFalse()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.AddMissingExportStrategyProvider(new OptionalStrategyProvider()));

            var instance = container.Locate<Option<IBasicService>>();

            Assert.False(instance.HasValue);
        }
        
        [Fact]
        public void OptionalBasicServiceHasValueTrue()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<BasicService, IBasicService>();
                c.AddMissingExportStrategyProvider(new OptionalStrategyProvider());
            });

            var instance = container.Locate<Option<IBasicService>>();

            Assert.True(instance.HasValue);
            Assert.IsType<BasicService>(instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }
    }
}
