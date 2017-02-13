using Grace.DependencyInjection;
using Grace.Tests.Classes.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class LifestyleAttributeTests
    {
        [Fact]
        public void SingletonPerObjectGraph_Attribute_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                                       ExportAttributedTypes().
                                       Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            var instance =
                container.Locate<TwoDependencyService<AttributedSingletonPerObjectGraphService, AttributedSingletonPerObjectGraphService>>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Dependency1);
            Assert.NotNull(instance.Dependency2);
            Assert.Same(instance.Dependency1, instance.Dependency2);

            var instance2 =
                container.Locate<TwoDependencyService<AttributedSingletonPerObjectGraphService, AttributedSingletonPerObjectGraphService>>();

            Assert.NotNull(instance2);
            Assert.NotNull(instance2.Dependency1);
            Assert.NotNull(instance2.Dependency2);
            Assert.Same(instance2.Dependency1, instance2.Dependency2);

            Assert.NotSame(instance.Dependency1, instance2.Dependency1);
        }

        [Fact]
        public void Singleton_Attribute_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                                       ExportAttributedTypes().
                                       Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            var instance = container.Locate<IAttributedSingletonService>();

            Assert.NotNull(instance);

            var instance2 = container.Locate<IAttributedSingletonService>();

            Assert.NotNull(instance2);

            Assert.Same(instance, instance2);
        }

        [Fact]
        public void SingletonPerScope_Attribute_Test()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<ExportAttributeTests>().
                                       ExportAttributedTypes().
                                       Where(TypesThat.AreInTheSameNamespaceAs<TestAttribute>()));

            using (var scope = container.BeginLifetimeScope())
            {
                
            }
        }
    }
}
