using System;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Lifestyle
{
    public class WeakSingletonTests
    {
        public class UniqueInstanceClass
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        [Fact]
        public void WeakSingleton_Return_Same_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<UniqueInstanceClass>().Lifestyle.WeakSingleton());

            Guid instanceGuid = TestWeakLifestyle(container);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Guid instance2Guid = TestWeakLifestyle(container);

            Assert.NotEqual(instanceGuid, instance2Guid);
        }

        private static Guid TestWeakLifestyle(DependencyInjectionContainer container)
        {
            var instance = container.Locate<UniqueInstanceClass>();

            Assert.Same(instance, container.Locate<UniqueInstanceClass>());

            return instance.Id;
        }
    }
}
