using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class WrappedImportKeyTests
    {
        class TypedMetadata
        {
            public string Foo { get; set; }
        }

        private DependencyInjectionContainer container = new();

        public WrappedImportKeyTests()
        {
            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Keyed")
                    .WithCtorParam<object>()
                    .LocateWithImportKey()
                    .ImportProperty(x => x.StringKey)
                    .LocateWithImportKey()
                    .WithMetadata("Foo", "Bar");

                c.Export(typeof(ServiceWithKeyWrapper<>));
            });
        }

        private void AssertInstance(ImportKeyService instance)
        {
            Assert.NotNull(instance);
            Assert.Equal("Keyed", instance.ObjectKey);
            Assert.Equal("Keyed", instance.StringKey);
        }

        [Fact]
        public void Func()
        {
            var func = container.Locate<Func<ImportKeyService>>(withKey: "Keyed");
            AssertInstance(func());
            
            var parent = container.Locate<ServiceWithKeyWrapper<Func<ImportKeyService>>>();
            AssertInstance(parent.Value());
        }

        [Fact]
        public void Lazy()
        {
            var lazy = container.Locate<Lazy<ImportKeyService>>(withKey: "Keyed");
            AssertInstance(lazy.Value);

            var parent = container.Locate<ServiceWithKeyWrapper<Lazy<ImportKeyService>>>();
            AssertInstance(parent.Value.Value);
        }

        [Fact]
        public void LazyMetadata()
        {
            var lazy = container.Locate<Lazy<ImportKeyService, IActivationStrategyMetadata>>(withKey: "Keyed");
            Assert.Equal("Bar", lazy.Metadata["Foo"]);
            AssertInstance(lazy.Value);

            var parent = container.Locate<ServiceWithKeyWrapper<Lazy<ImportKeyService, IActivationStrategyMetadata>>>();
            Assert.Equal("Bar", parent.Value.Metadata["Foo"]);
            AssertInstance(parent.Value.Value);
        }

        [Fact]
        public void Meta()
        {
            var meta = container.Locate<Meta<ImportKeyService>>(withKey: "Keyed");
            Assert.Equal("Bar", meta.Metadata["Foo"]);
            AssertInstance(meta.Value);

            var parent = container.Locate<ServiceWithKeyWrapper<Meta<ImportKeyService>>>();
            Assert.Equal("Bar", parent.Value.Metadata["Foo"]);
            AssertInstance(parent.Value.Value);
        }

        [Fact]
        public void MetaTyped()
        {
            var meta = container.Locate<Meta<ImportKeyService, TypedMetadata>>(withKey: "Keyed");
            Assert.Equal("Bar", meta.Metadata.Foo);
            AssertInstance(meta.Value);

            var parent = container.Locate<ServiceWithKeyWrapper<Meta<ImportKeyService, TypedMetadata>>>();
            Assert.Equal("Bar", parent.Value.Metadata.Foo);
            AssertInstance(parent.Value.Value);
        }

        [Fact]
        public void Owned()
        {
            var owned = container.Locate<Owned<ImportKeyService>>(withKey: "Keyed");
            AssertInstance(owned.Value);

            var parent = container.Locate<ServiceWithKeyWrapper<Owned<ImportKeyService>>>();
            AssertInstance(parent.Value.Value);
        }

        [Fact]
        public void Scoped()
        {
            using (var scoped = container.Locate<Scoped<ImportKeyService>>(withKey: "Keyed"))
            {
                AssertInstance(scoped.Instance);
            }

            var parent = container.Locate<ServiceWithKeyWrapper<Scoped<ImportKeyService>>>();
            using (var scoped = parent.Value)
            {
                AssertInstance(scoped.Instance);
            }
        }

        [Fact]
        public void MultipleWrappers()
        {
            var located = container.Locate<Meta<Func<Scoped<ImportKeyService>>>>(withKey: "Keyed");

            Assert.Equal("Bar", located.Metadata["Foo"]);

            using (var scoped = located.Value())
            {
                AssertInstance(scoped.Instance);
            }

            var parent = container.Locate<ServiceWithKeyWrapper<Meta<Func<Scoped<ImportKeyService>>>>>();
            located = parent.Value;

            Assert.Equal("Bar", located.Metadata["Foo"]);

            using (var scoped = located.Value())
            {
                AssertInstance(scoped.Instance);
            }
        }
    }
}