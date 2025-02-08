using Optional;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using System;
using Xunit;

namespace Grace.Tests.ThirdParty.Optional
{
    public class OptionalTests
    {
        private DependencyInjectionContainer container = new();

        public OptionalTests()
        {
            container.Configure(c =>
            {                
                c.ExportWrapper(new OptionalWrapperStrategy(c.OwningScope));
            });
        }

        [Fact]
        public void OptionalIntHasValueFalse()
        {
            var instance = container.Locate<Option<int>>();

            Assert.False(instance.HasValue);
        }

        [Fact]
        public void OptionalIntHasValueTrue()
        {
            container.Configure(c => c.ExportInstance(5));

            var instance = container.Locate<Option<int>>();

            Assert.True(instance.HasValue);
            Assert.Equal(5, instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceHasValueFalse()
        {
            var instance = container.Locate<Option<IBasicService>>();

            Assert.False(instance.HasValue);
        }
        
        [Fact]
        public void OptionalBasicServiceHasValueTrue()
        {
            container.Configure(c => c.ExportAs<BasicService, IBasicService>());

            var instance = container.Locate<Option<IBasicService>>();

            Assert.True(instance.HasValue);
            Assert.IsType<BasicService>(instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndIsSatisfied()
        {
            container.Configure(c =>
            {
                c.Export<BasicService>().AsKeyed<IBasicService>("basic");
            });

            var instance = container.Locate<Option<IBasicService>>(withKey: "basic");

            Assert.IsType<BasicService>(instance.ValueOr(() => throw new Exception("Not supposed to hit this")));
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndNotIsSatisfied()
        {
            var instance = container.Locate<Option<IBasicService>>(withKey: "basic");

            Assert.False(instance.HasValue);
        }

        [Fact]
        public void OptionalBasicServiceWithKeyAndScopedService()
        {
            container.Configure(c =>
            {
                c.Export<BasicService>().AsKeyed<IBasicService>("basic").Lifestyle.SingletonPerScope();
            });

            var scope = container.CreateChildScope();

            var instance1 = scope.Locate<Option<IBasicService>>(withKey: "basic");
            var instance2 = scope.Locate<Option<IBasicService>>(withKey: "basic");

            var value1 = instance1.ValueOr(() => throw new Exception("Not supposed to hit this"));
            Assert.NotNull(value1);
            var value2 = instance2.ValueOr(() => throw new Exception("Not supposed to hit this"));
            Assert.NotNull(value2);

            Assert.Same(value1, value2);
        }
    }
}
