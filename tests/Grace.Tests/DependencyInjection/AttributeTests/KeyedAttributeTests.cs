using System.Reflection;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.AttributeTests
{
    public class KeyedAttributeTests
    {
        [Fact]
        public void KeyedAttributeTest()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(ImportKeyedMultiple));

            container.Configure(c => c
                .ExportAssembly(assembly)
                .ExportAttributedTypes()
                .Where(TypesThat.AreInTheSameNamespaceAs(typeof(ImportKeyedMultiple)))
            );

            var instance = container.Locate<ImportKeyedMultiple>();

            Assert.NotNull(instance);
            Assert.Equal("A", instance.ServiceA.SomeMethod());
            Assert.Equal("B", instance.ServiceB.SomeMethod());
            Assert.Equal("A", instance.AdaptedServiceA.SomeMethod());
            Assert.Equal("B", instance.AdaptedServiceB.SomeMethod());
            Assert.Equal("A", instance.ServiceA.LocatedKey);
            Assert.Equal("B", instance.ServiceB.LocatedKey);
        }

        [Fact]
        public void AnyKeyAttributeTest()
        {
            var container = new DependencyInjectionContainer();

            var assembly = Assembly.GetAssembly(typeof(IAnyKeyService));

            container.Configure(c => c
                .ExportAssembly(assembly)
                .ExportAttributedTypes()
                .Where(TypesThat.AreInTheSameNamespaceAs(typeof(IAnyKeyService)))
            );

            var a = container.Locate<IAnyKeyService>(withKey: "A"); // ExportKeyedType("A")
            var b = container.Locate<IAnyKeyService>(withKey: "B"); // ExportAnyKeyedType

            Assert.NotNull(a);
            Assert.IsType<AnyKeyServiceA>(a);
            Assert.NotNull(b);
            Assert.IsType<AnyKeyServiceB>(b);
        }
    }
}
