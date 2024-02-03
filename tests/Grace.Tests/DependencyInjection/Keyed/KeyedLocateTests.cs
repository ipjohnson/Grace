using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.Tests.Classes.Generics;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Keyed
{
    public class KeyedLocateTests
    {
        [Fact]
        public void Export_With_Key_Locate_From_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>("B");
            });

            var instanceA = container.Locate<ISimpleObject>(withKey: "A");
            var instanceB = container.Locate<ISimpleObject>(withKey: "B");

            Assert.IsType<SimpleObjectA>(instanceA);
            Assert.IsType<SimpleObjectB>(instanceB);
        }

        [Fact]
        public void Export_With_Key_LocateAll_From_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>(ImportKey.Any);
                c.Export<SimpleObjectC>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectD>().As<ISimpleObject>();
            });
            
            var instances = container.LocateAll<ISimpleObject>(withKey: "A");

            Assert.Equal(3, instances.Count);
            // Without comparer, LocateAll default order is:
            // non-any keys first, then in reverse export order (latest registration comes first)
            Assert.IsType<SimpleObjectC>(instances[0]);
            Assert.IsType<SimpleObjectA>(instances[1]);
            Assert.IsType<SimpleObjectB>(instances[2]);
        }

        [Fact]
        public void Export_With_Key_Import_With_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>("B");
                c.Export<ImportSingleSimpleObject>().AsKeyed<ImportSingleSimpleObject>("A").WithCtorParam<ISimpleObject>().LocateWithKey("A");
                c.Export<ImportSingleSimpleObject>().AsKeyed<ImportSingleSimpleObject>("B").WithCtorParam<ISimpleObject>().LocateWithKey("B");
            });

            var instanceA = container.Locate<ImportSingleSimpleObject>(withKey: "A");
            var instanceB = container.Locate<ImportSingleSimpleObject>(withKey: "B");

            Assert.NotNull(instanceA);
            Assert.NotNull(instanceB);

            Assert.IsType<SimpleObjectA>(instanceA.SimpleObject);
            Assert.IsType<SimpleObjectB>(instanceB.SimpleObject);
        }

        [Fact]
        public void Export_With_Import_Keyed_IEnumerable()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>(ImportKey.Any);
                c.Export<SimpleObjectC>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectD>().AsKeyed<ISimpleObject>("D");
                c.Export<ImportMultipleSimpleObjects>().WithCtorParam<ISimpleObject[]>().LocateWithKey("A");
            });

            var instance = container.Locate<ImportMultipleSimpleObjects>();
            Assert.NotNull(instance);

            var simples = instance.SimpleObjects;
            Assert.NotNull(simples);
            // Without comparer, default order of nested IEnumerable or Array is:
            // by priority (higher first), then export order.
            Assert.IsType<SimpleObjectA>(simples[0]);
            Assert.IsType<SimpleObjectB>(simples[1]);
            Assert.IsType<SimpleObjectC>(simples[2]);
        }

        [Fact]
        public void KeyedLocateDelegate_Create()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>("B");
            });

            var keyedDelegate = container.Locate<KeyedLocateDelegate<string, ISimpleObject>>();

            var instanceA = keyedDelegate("A");
            var instanceB = keyedDelegate("B");

            Assert.IsType<SimpleObjectA>(instanceA);
            Assert.IsType<SimpleObjectB>(instanceB);
        }

        [Fact]
        public void ExportFactory_Import_Keyed()
        {
            var container = new DependencyInjectionContainer();
            
            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
                c.ExportFactory(([Import(Key = "A")] ISimpleObject simpleObject) 
                    => new ImportSingleSimpleObject(simpleObject));
            });

            var instance = container.Locate<ImportSingleSimpleObject>();

            Assert.NotNull(instance);
            Assert.IsType<SimpleObjectA>(instance.SimpleObject);
        }

        [Fact]
        public void ExportFactory_AsKeyed_Any()
        {
            var container = new DependencyInjectionContainer();
            
            container.Configure(c =>
            {
                c.ExportFactory(([ImportKey] string key) => $"Key is {key}")
                    .AsKeyed<string>(ImportKey.Any);
            });

            var instance = container.Locate<string>(withKey: "A");

            Assert.Equal("Key is A", instance);
        }        

        [Fact]
        public void Func_Keyed_Result()
        {
            var container = new DependencyInjectionContainer();
            
            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>("A");
            });

            var func = container.Locate<Func<ISimpleObject>>(withKey: "A");

            Assert.IsType<SimpleObjectA>(func());         
        }

        [Fact]
        public void AsKeyedStringTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportFactory(() => "Hello");
                c.ExportFactory(() => "HelloAgain").AsKeyed<string>("Key");
            });

            Assert.Equal("Hello", container.Locate<string>());
            Assert.Equal("HelloAgain", container.Locate<string>(withKey: "Key"));
        }

        [Fact]
        public void AsKeyedBasicTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>('A');
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>('B');
                c.Export<SimpleObjectC>().AsKeyed<ISimpleObject>('C');
                c.Export<SimpleObjectD>().AsKeyed<ISimpleObject>('D');
                c.Export<SimpleObjectE>().AsKeyed<ISimpleObject>('E');
            });

            for (var locateChar = 'A'; locateChar < 'F'; locateChar++)
            {
                var simpleObject = container.Locate<ISimpleObject>(withKey: locateChar);

                Assert.NotNull(simpleObject);
                Assert.EndsWith(locateChar.ToString(), simpleObject.GetType().FullName);
            }
        }

        [Fact]
        public void AsKeyed_Override()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().AsKeyed<ISimpleObject>('A');
                c.Export<SimpleObjectB>().AsKeyed<ISimpleObject>('A');
            });

            var instance = container.Locate<ISimpleObject>(withKey: 'A');

            Assert.NotNull(instance);
            Assert.IsType<SimpleObjectB>(instance);
        }

        [Fact]
        public void Value_Parameter_Uses_Key()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance(5).AsKeyed<int>("value");
            });

            var instance = container.Locate<DependentService<int>>();

            Assert.NotNull(instance);
            Assert.Equal(5, instance.Value);
        }

        public class DependentEnumerable
        {
            public DependentEnumerable(IEnumerable<IDependentService<IBasicService>> services)
            {
                Services = services;
            }

            public IEnumerable<IDependentService<IBasicService>> Services { get; }
        }

        [Fact]
        public void Keyed_Generic_Value()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<BasicService, IBasicService>();
                c.Export(typeof(DependentService<>)).AsKeyed(typeof(IDependentService<>), 'A');
                c.Export<DependentEnumerable>()
                    .WithCtorParam<IEnumerable<IDependentService<IBasicService>>>().LocateWithKey(new[] { 'A' });
            });

            var instance = container.Locate<DependentEnumerable>();

            Assert.NotNull(instance);
            var array = instance.Services.ToArray();

            Assert.Single(array);
            Assert.IsType<DependentService<IBasicService>>(array[0]);
        }

        [Fact]
        public void Keyed_Wrapped_Generic_Value()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportAs<BasicService, IBasicService>();
                c.Export(typeof(DependentService<>)).AsKeyed(typeof(IDependentService<>), 'A');
            });

            var owned = container.Locate<Owned<IDependentService<IBasicService>>>(withKey: 'A');

            Assert.NotNull(owned);            
            Assert.IsType<DependentService<IBasicService>>(owned.Value);
            Assert.NotNull(owned.Value.Value);
        }

        [Fact]
        public void Keyed_And_NonKeyed_With_Different_Lifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().Lifestyle.SingletonPerNamedScope("CustomScopeName");
                c.Export<DisposableService>().AsKeyed<DisposableService>("TransientKey").ExternallyOwned();
            });

            bool disposedService = false;
            bool disposedTransient = false;

            using (var scope = container.BeginLifetimeScope("CustomScopeName"))
            {
                var service = scope.Locate<DisposableService>();

                Assert.Same(service, scope.Locate<DisposableService>());

                service.Disposing += (sender, args) => disposedService = true;

                var transientService = scope.Locate<DisposableService>(withKey: "TransientKey");

                Assert.NotSame(service, transientService);

                transientService.Disposing += (sender, args) => disposedTransient = true;
            }

            Assert.True(disposedService);
            Assert.False(disposedTransient);
        }

        public class FactoryClass
        {
            private readonly KeyedLocateDelegate<string, DisposableService> _createDelegate;

            public FactoryClass(KeyedLocateDelegate<string, DisposableService> createDelegate)
            {
                _createDelegate = createDelegate;
            }

            public DisposableService CreateService()
            {
                return _createDelegate("TransientKey");
            }
        }

        [Fact]
        public void Keyed_And_NonKeyed_Factory_With_Different_Lifestyle()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().Lifestyle.SingletonPerNamedScope("CustomScopeName");
                c.Export<DisposableService>().AsKeyed<DisposableService>("TransientKey").ExternallyOwned();
            });

            bool disposedService = false;
            bool disposedTransient = false;

            using (var scope = container.BeginLifetimeScope("CustomScopeName"))
            {
                var service = scope.Locate<DisposableService>();

                Assert.Same(service, scope.Locate<DisposableService>());

                service.Disposing += (sender, args) => disposedService = true;

                var factory = scope.Locate<FactoryClass>();

                var transientService = factory.CreateService();

                Assert.NotSame(service, transientService);

                transientService.Disposing += (sender, args) => disposedTransient = true;
            }

            Assert.True(disposedService);
            Assert.False(disposedTransient);
        }

        [Fact]
        public void KeyedWithGeneric()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export(typeof(ImportGenericService<>)).AsKeyed(typeof(IImportGenericService<>), "A");
                c.Export<BasicService>().As<IBasicService>();
            });

            var service = container.Locate<IImportGenericService<IBasicService>>(withKey: "A");

            Assert.NotNull(service);
            Assert.NotNull(service.Value);
            Assert.IsType<BasicService>(service.Value);
        }

        public class FuncFactoryClass(Func<DisposableService> func)
        {
           public DisposableService CreateService() => func();
        }

        [Fact]
        public void Keyed_Factory_And_NonKeyed_With_Different_Lifestyle()
        {
           var container = new DependencyInjectionContainer();

           container.Configure(c =>
           {
               c.Export<DisposableService>().Lifestyle.SingletonPerNamedScope("CustomScopeName");
               c.Export<DisposableService>().AsKeyed<DisposableService>("TransientKey").ExternallyOwned();
               c.Export<FuncFactoryClass>().WithCtorParam<Func<DisposableService>>().LocateWithKey("TransientKey");
           });

           bool disposedService = false;
           bool disposedTransient = false;

           using (var scope = container.BeginLifetimeScope("CustomScopeName"))
           {
               var service = scope.Locate<DisposableService>();

               Assert.Same(service, scope.Locate<DisposableService>());

               service.Disposing += (sender, args) => disposedService = true;

               var factory = scope.Locate<FuncFactoryClass>();

               var transientService = factory.CreateService();

               Assert.NotSame(service, transientService);

               transientService.Disposing += (sender, args) => disposedTransient = true;
           }

           Assert.True(disposedService);
           Assert.False(disposedTransient);
        }


        [Fact]
        public void Keyed_As_Way_Of_Picking_Based_On_Scope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().AsKeyed<IBasicService>("Default");
                c.Export<CustomBasicService>().AsKeyed<IBasicService>("Custom");
                c.ExportFactory<IExportLocatorScope, IBasicService>(
                    scope => scope.Locate<IBasicService>(withKey: scope.ScopeName == "CustomScope" ? "Custom" : "Default"));
            });

            var instance = container.Locate<DependentService<IBasicService>>();

            Assert.IsType<BasicService>(instance.Value);

            using (var scope = container.BeginLifetimeScope("SomeScope"))
            {
                instance = scope.Locate<DependentService<IBasicService>>();

                Assert.IsType<BasicService>(instance.Value);
            }

            using (var scope = container.BeginLifetimeScope("CustomScope"))
            {
                instance = scope.Locate<DependentService<IBasicService>>();

                Assert.IsType<CustomBasicService>(instance.Value);
            }
        }
    }
}
