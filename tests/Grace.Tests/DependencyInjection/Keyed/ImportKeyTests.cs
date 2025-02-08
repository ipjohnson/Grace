using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class ImportKeyTests
    {
        [Fact]
        public void Inject_Import_Key()
        {
            var container = new DependencyInjectionContainer();

            var guidKey = Guid.NewGuid();

            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Ctor")
                    .ImportConstructor(() => new ImportKeyService());

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("CtorObject")
                    .ImportConstructor(() => new ImportKeyService(null))
                    .WithCtorParam<object>()
                    .LocateWithImportKey();

                // There seems to be no way today to inject a key into a method using Fluent API.
                // Notice that Arg expressions in ImportMethod(..) are not processed.
                // Neither is there an equivalent to WithCtorParam<T>(), such as WithMethodParam<T>().
                // c.Export<ImportKeyService>().AsKeyed<ImportKeyService>("Method")
                //    .ImportMethod(s => s.ImportMethod(Arg.ImportKey<object>()));

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("PropertyObject")
                    .ImportProperty(s => s.ObjectKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("PropertyString")
                    .ImportProperty(s => s.StringKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(42)
                    .ImportProperty(s => s.IntKey)
                    .LocateWithImportKey()
                    .ImportProperty(s => s.ObjectKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(guidKey)
                    .ImportProperty(s => s.StringKey)
                    .LocateWithImportKey()
                    .ImportProperty(s => s.IntKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .As<ImportKeyService>()
                    .ImportProperty(s => s.ObjectKey)
                    .LocateWithImportKey()
                    .ImportProperty(s => s.IntKey)
                    .LocateWithImportKey();
            });

            var instance = container.Locate<ImportKeyService>(withKey: "Ctor");
            Assert.Null(instance.ObjectKey);

            instance = container.Locate<ImportKeyService>(withKey: "CtorObject");
            Assert.Equal("CtorObject", instance.ObjectKey);

            // See comment above regarding Fluent API not supporting keyed imports on Method parameters
            // instance = container.Locate<ImportKeyService>(withKey: "Method");
            // Assert.Equal("Method", instance.ObjectKey);

            instance = container.Locate<ImportKeyService>(withKey: "PropertyObject");
            Assert.Equal("PropertyObject", instance.ObjectKey);

            instance = container.Locate<ImportKeyService>(withKey: "PropertyString");
            Assert.Equal("PropertyString", instance.StringKey);

            instance = container.Locate<ImportKeyService>(withKey: 42);
            Assert.Equal(42, instance.IntKey);
            Assert.Equal(42, instance.ObjectKey); // Boxing to ref type

            // This behavior, including the choice of InvalidOperationException specifically,
            // is meant to match the specifications of MS DI interfaces.
            // Changing this will break Grace.DependencyInjection.Extensions.
            Assert.ThrowsAny<InvalidOperationException>(() => 
                container.Locate<ImportKeyService>(withKey: guidKey));

            instance = container.Locate<ImportKeyService>();
            Assert.Null(instance.ObjectKey); // Non-keyed import injects null (target is ref type)
            Assert.Equal(0, instance.IntKey); // Non-keyed import injects default (target is value type)
        }

        [Fact]
        public void Inject_Nested_Import_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyServiceWrapper>()
                    .AsKeyed<ImportKeyServiceWrapper>("Parent")
                    .ImportConstructor(() => new ImportKeyServiceWrapper(null))
                    .WithCtorParam<ImportKeyService>()
                    .LocateWithKey("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey();
            });

            var instance = container.Locate<ImportKeyServiceWrapper>(withKey: "Parent");

            Assert.NotNull(instance);
            Assert.Equal("Parent", instance.ObjectKey);
            Assert.NotNull(instance.Service);
            Assert.Equal("Child", instance.Service.ObjectKey);
        }

        [Fact]
        public void SingletonPerScope_Imported_Key()
        {
            // This test is critical because there is unique handling of the activation delegate in SingeltonPerScopeLifestyle.

            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Keyed")
                    .WithCtorParam<object>()
                    .Named("key")
                    .LocateWithImportKey()
                    .ImportProperty(x => x.StringKey)
                    .LocateWithImportKey()
                    .Lifestyle
                    .SingletonPerScope();
            });

            var instance = container.Locate<ImportKeyService>(withKey: "Keyed");

            Assert.NotNull(instance);
            Assert.Equal("Keyed", instance.ObjectKey);
            Assert.Equal("Keyed", instance.StringKey);
        }

        [Fact]
        public void Nested_Singleton_Imported_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyServiceWrapper>()
                    .AsKeyed<ImportKeyServiceWrapper>("Parent")                    
                    .ImportConstructor(() => new ImportKeyServiceWrapper(null))
                    .WithCtorParam<ImportKeyService>()
                    .LocateWithKey("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.Singleton();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.Singleton();
            });

            var instance = container.Locate<ImportKeyServiceWrapper>(withKey: "Parent");

            Assert.NotNull(instance);
            Assert.Equal("Parent", instance.ObjectKey);
            Assert.NotNull(instance.Service);
            Assert.Equal("Child", instance.Service.ObjectKey);
        }

        [Fact]
        public void Nested_SingletonPerNamedScope_Imported_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyServiceWrapper>()
                    .AsKeyed<ImportKeyServiceWrapper>("Parent")
                    .ImportConstructor(() => new ImportKeyServiceWrapper(null))
                    .WithCtorParam<ImportKeyService>()
                    .LocateWithKey("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.SingletonPerNamedScope("ScopeName");

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.SingletonPerNamedScope("ScopeName");
            });

            var instance = container
                .CreateChildScope(scopeName: "ScopeName")
                .Locate<ImportKeyServiceWrapper>(withKey: "Parent");

            Assert.NotNull(instance);
            Assert.Equal("Parent", instance.ObjectKey);
            Assert.NotNull(instance.Service);
            Assert.Equal("Child", instance.Service.ObjectKey);
        }

        [Fact]
        public void Nested_SingletonPerScope_Imported_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyServiceWrapper>()
                    .AsKeyed<ImportKeyServiceWrapper>("Parent")
                    .ImportConstructor(() => new ImportKeyServiceWrapper(null))
                    .WithCtorParam<ImportKeyService>()
                    .LocateWithKey("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.SingletonPerScope();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey()
                    .Lifestyle.SingletonPerScope();
            });

            var instance = container.Locate<ImportKeyServiceWrapper>(withKey: "Parent");

            Assert.NotNull(instance);
            Assert.Equal("Parent", instance.ObjectKey);
            Assert.NotNull(instance.Service);
            Assert.Equal("Child", instance.Service.ObjectKey);
        }
    }
}
