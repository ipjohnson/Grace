using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class CollectionsKeyTests
    {
        private DependencyInjectionContainer container = new();

        public CollectionsKeyTests()
        {
            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Keyed")
                    .WithCtorParam<object>()
                    .LocateWithImportKey()
                    .WithCtorParam<int>()
                    .Named("extraParameter")
                    .DefaultValue(1)
                    .ImportProperty(x => x.StringKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(ImportKey.Any)
                    .WithCtorParam<object>()
                    .LocateWithImportKey()
                    .WithCtorParam<int>()
                    .Named("extraParameter")
                    .DefaultValue(2)
                    .ImportProperty(x => x.StringKey)
                    .LocateWithImportKey();

                c.Export(typeof(ServiceWithKeyWrapper<>));
            });
        }

        private static void AssertInstance1(ImportKeyService instance)
        {
            Assert.Equal("Keyed", instance.ObjectKey);
            Assert.Equal("Keyed", instance.StringKey);
            Assert.Equal(1, instance.IntKey);
        }

        private static void AssertInstance2(ImportKeyService instance)
        {
            Assert.Equal("Keyed", instance.ObjectKey);
            Assert.Equal("Keyed", instance.StringKey);
            Assert.Equal(2, instance.IntKey);
        }

        [Fact]
        public void CanLocateMissing()
        {
            var canLocate = container.CanLocate(typeof(IEnumerable<SimpleObjectA>), key: "missing");
            Assert.True(canLocate);

            var list = container.Locate<SimpleObjectA[]>(withKey: "missing");
            Assert.Empty(list);
        }

        [Fact]
        public void LocateAll()
        {
            var all = container.LocateAll<ImportKeyService>(withKey: "Keyed");
            Assert.Collection(all, AssertInstance1, AssertInstance2);            
        }

        [Fact]
        public void IEnumerable()
        {
            var enumerable = container.Locate<IEnumerable<ImportKeyService>>(withKey: "Keyed");
            Assert.Collection(enumerable, AssertInstance1, AssertInstance2);

            var parent = container.Locate<ServiceWithKeyWrapper<IEnumerable<ImportKeyService>>>();
            Assert.Collection(parent.Value, AssertInstance1, AssertInstance2);
        }

        [Fact]
        public void Array()
        {
            var array = container.Locate<ImportKeyService[]>(withKey: "Keyed");
            Assert.Collection(array, AssertInstance1, AssertInstance2);

            var parent = container.Locate<ServiceWithKeyWrapper<ImportKeyService[]>>();
            Assert.Collection(parent.Value, AssertInstance1, AssertInstance2);
        }

        [Fact]
        public void List()
        {
            var list = container.Locate<List<ImportKeyService>>(withKey: "Keyed");
            Assert.Collection(list, AssertInstance1, AssertInstance2);

            var parent = container.Locate<ServiceWithKeyWrapper<List<ImportKeyService>>>();
            Assert.Collection(parent.Value, AssertInstance1, AssertInstance2);
        }
    }
}