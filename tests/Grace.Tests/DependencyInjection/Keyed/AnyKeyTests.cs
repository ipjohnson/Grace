using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Exceptions;
using Grace.Tests.Classes.Simple;
using NSubstitute;
using Xunit;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class AnyKeyTests
    {        
        [Fact]
        public void Prefer_Exact_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>("SpecificKey");
                c.Export<MultipleService2>().AsKeyed<IMultipleService>(ImportKey.Any);
            });

            var instance1 = container.Locate<IMultipleService>(withKey: "SpecificKey");
            Assert.NotNull(instance1);
            Assert.IsType<MultipleService1>(instance1);

            var instance2 = container.Locate<IMultipleService>(withKey: "OtherKey");
            Assert.NotNull(instance2);
            Assert.IsType<MultipleService2>(instance2);
        }

        [Fact]
        public void Does_Not_Match_Unkeyed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsKeyed<IMultipleService>(ImportKey.Any);
            });

            Assert.Throws<LocateException>(() => container.Locate<IMultipleService>());
        }
        
        [Fact]
        public void Inject_Dynamic_Import_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyServiceWrapper>()
                    .AsKeyed<ImportKeyServiceWrapper>(ImportKey.Any)
                    .ImportConstructor(() => new ImportKeyServiceWrapper(null))
                    .WithCtorParam<ImportKeyService>()
                    .LocateWithKey("Child")
                    .ImportProperty(x => x.ObjectKey)
                    .LocateWithImportKey();

                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(ImportKey.Any)
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
        public void Singletons_Per_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(ImportKey.Any)
                    .Lifestyle
                    .Singleton();
            });

            var instance1 = container.Locate<ImportKeyService>(withKey: "Key1");
            Assert.NotNull(instance1);
            var instance2 = container.Locate<ImportKeyService>(withKey: "Key2");
            Assert.NotNull(instance2);
            var instance3 = container.Locate<ImportKeyService>(withKey: "Key1");
            Assert.NotNull(instance3);

            Assert.NotSame(instance1, instance2);
            Assert.Same(instance1, instance3);
        }

        [Fact]
        public void SingletonsPerScope_Per_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<ImportKeyService>()
                    .AsKeyed<ImportKeyService>(ImportKey.Any)
                    .Lifestyle
                    .SingletonPerScope();
            });

            var instance1 = container.Locate<ImportKeyService>(withKey: "Key1");
            Assert.NotNull(instance1);
            var instance2 = container.Locate<ImportKeyService>(withKey: "Key2");
            Assert.NotNull(instance2);
            var instance3 = container.Locate<ImportKeyService>(withKey: "Key1");
            Assert.NotNull(instance3);

            var scope = container.CreateChildScope();
            var instance4 = scope.Locate<ImportKeyService>(withKey: "Key1");
            Assert.NotNull(instance4);

            Assert.NotSame(instance1, instance2);
            Assert.Same(instance1, instance3);
            Assert.NotSame(instance1, instance4);
        }

        [Fact]
        public void KeyDependent_IDisposable_Scope_Is_Root()
        {
            // Test case for #316

            var service = Substitute.For<IDisposable>();

            var container = new DependencyInjectionContainer();            

            container.Configure(c => 
            {
                c.ExportFactory(([ImportKey] string key) => service)
                    .AsKeyed<IDisposable>("keyed")
                    .Lifestyle.Singleton();
            });

            using (var scope = container.CreateChildScope())
            {
                var located = scope.Locate<IDisposable>(withKey: "keyed");
                Assert.Equal(service, located);
            }
            
            service.DidNotReceive().Dispose();

            container.Dispose();

            service.Received(1).Dispose();
        }
    }
}
