using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.Diagnostics;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
    public class AdvancedContainerTests
    {
        #region Metdata tests

        [Fact]
        public void LocateAllWithMetadataTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
            });
            var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata");

            Assert.NotNull(list);

            Assert.Equal(5, list.Count());
        }

        [Fact]
        public void LocateAllWithMetadataFiltered()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithMetadata("Metadata", "Group1");
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithMetadata("Metadata", "Group2");
            });

            var list = container.LocateAllWithMetadata<ISimpleObject>("Metadata", "Group1");

            Assert.NotNull(list);

            Assert.Equal(3, list.Count());

        }

        #endregion

        #region Keyed tests

        [Fact]
        public void KeyedTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                      {
                                          c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                                          c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                                          c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                                          c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                                          c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
                                      });

            ISimpleObject simpleObject = container.Locate<ISimpleObject>(withKey: 3);

            Assert.NotNull(simpleObject);

            Assert.IsType<SimpleObjectC>(simpleObject);
        }

        [Fact]
        public void MultipleKeyLocate()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            ISimpleObject simpleObject = container.Locate<ISimpleObject>(withKey: new[] { 1, 2, 3 });

            Assert.NotNull(simpleObject);

            Assert.IsType<SimpleObjectC>(simpleObject);
        }

        [Fact]
        public void LocateAllKeyed()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            var simpleObjects = container.LocateAll<ISimpleObject>(withKey: new[] { 1, 2, 3 });

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count);
        }

        [Fact]
        public void LocateEmptyCollectionByKey()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            var simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(0, simpleObjects.Count);
        }

        [Fact]
        public void LocateKeyedLazy()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            Lazy<ISimpleObject> simpleLazy = container.Locate<Lazy<ISimpleObject>>(withKey: 3);

            Assert.NotNull(simpleLazy);
            Assert.NotNull(simpleLazy.Value);
            Assert.IsType<SimpleObjectC>(simpleLazy.Value);
        }

        [Fact]
        public void LocateKeyedEnumerable()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            var list = container.Locate<IEnumerable<ISimpleObject>>(withKey: new[] { 3, 4, 5 });

            Assert.NotNull(list);

            Assert.Equal(3, list.Count());
        }

        [Fact]
        public void LocateKeyedMetadata()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1).WithMetadata("Test", "Group");
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2).WithMetadata("Test", "Group");
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3).WithMetadata("Test", "Group");
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4).WithMetadata("Test", "Group");
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5).WithMetadata("Test", "Group");
            });

            Meta<ISimpleObject> simpleMeta = container.Locate<Meta<ISimpleObject>>(withKey: 3);

            Assert.NotNull(simpleMeta);
            Assert.NotNull(simpleMeta.Value);
            Assert.IsType<SimpleObjectC>(simpleMeta.Value);
            Assert.Equal("Group", simpleMeta.Metadata["Test"]);
        }

        [Fact]
        public void LocateKeyedEnumerableLazy()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            var list = container.Locate<IEnumerable<Lazy<ISimpleObject>>>(withKey: new[] { 1, 2, 3 });

            Assert.NotNull(list);
            Assert.Equal(3, list.Count());

            foreach (Lazy<ISimpleObject> lazy in list)
            {
                Assert.NotNull(lazy.Value);
            }
        }

        [Fact]
        public void ImportKeyedLazy()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                      {
                                          c.Export<BasicService>().As<IBasicService>();
                                          c.Export<OtherBasicService>().As<IBasicService>().WithKey(5);
                                          c.Export<LazyImportService>().
                                             ByInterfaces().
                                             WithCtorParam<Lazy<IBasicService>>().LocateWithKey(5);
                                      });

            LazyImportService importService = container.Locate<LazyImportService>();

            Assert.NotNull(importService);
            Assert.NotNull(importService.BasicService);
            Assert.IsType<OtherBasicService>(importService.BasicService);
        }

        [Fact]
        public void ImportIEnumerableKeyed()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                      {
                                          c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey("A");
                                          c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey("B");
                                          c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey("C");
                                          c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey("D");
                                          c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey("E");
                                          c.Export<ImportObservableCollectionService>().
                                             WithCtorParam<IEnumerable<ISimpleObject>>().LocateWithKey(new[] { "A", "B", "C" });
                                      });

            ImportObservableCollectionService service = container.Locate<ImportObservableCollectionService>();

            Assert.NotNull(service);
            Assert.Equal(3, service.Count);
        }

        [Fact]
        public void KeyedLocateDelegate()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            KeyedLocateDelegate<int, ISimpleObject> locateDelegate = container.Locate<KeyedLocateDelegate<int, ISimpleObject>>();

            Assert.NotNull(locateDelegate);

            ISimpleObject simpleObject = locateDelegate(3);

            Assert.NotNull(simpleObject);
            Assert.IsType<SimpleObjectC>(simpleObject);
        }


        [Fact]
        public void ImportKeyedLocateDelegateTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>().WithKey(1);
                c.Export<SimpleObjectB>().As<ISimpleObject>().WithKey(2);
                c.Export<SimpleObjectC>().As<ISimpleObject>().WithKey(3);
                c.Export<SimpleObjectD>().As<ISimpleObject>().WithKey(4);
                c.Export<SimpleObjectE>().As<ISimpleObject>().WithKey(5);
            });

            ImportKeyedLocateDelegate locateDelegate = container.Locate<ImportKeyedLocateDelegate>();

            Assert.NotNull(locateDelegate);

            ISimpleObject simpleObject = locateDelegate.Locate(3);

            Assert.NotNull(simpleObject);
            Assert.IsType<SimpleObjectC>(simpleObject);
        }

        [Fact]
        public void BulkWithKeyTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterface<ISimpleObject>()
                                      .WithKey(t => t.Name.Last()));

            ISimpleObject simpleA = container.Locate<ISimpleObject>(withKey: 'A');

            Assert.NotNull(simpleA);
            Assert.IsType<SimpleObjectA>(simpleA);

            var simpleObjects =
                container.LocateAll<ISimpleObject>(withKey: new[] { 'A', 'C', 'E' });

            Assert.Equal(3, simpleObjects.Count);

            Assert.Equal(5, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void BulkLocateWithKey()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterface<ISimpleObject>()
                                      .WithKey(t => t.Name.Last()));

            container.Configure(c =>
                                {
                                    foreach (char ch in "ABCDE")
                                    {
                                        c.Export<ImportSingleSimpleObject>()
                                         .WithKey(ch)
                                         .WithCtorParam<ISimpleObject>()
                                         .LocateWithKey(ch);
                                    }
                                });

            ImportSingleSimpleObject single = container.Locate<ImportSingleSimpleObject>(withKey: 'A');

            Assert.NotNull(single);
            Assert.IsType<SimpleObjectA>(single.SimpleObject);

            Assert.Equal(3, container.LocateAll<ImportSingleSimpleObject>(withKey: new[] { 'A', 'C', 'E' }).Count);
        }

        [Fact]
        public void ImportKeyedBulkTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterface<ISimpleObject>()
                                      .WithKey(t => t.Name.Last()));

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<IImportKeyed>())
                                      .WithKey(t => t.Name.Last())
                                      .WithCtorParam<ISimpleObject>().LocateWithKey(t => t.Name.Last()));

            IImportKeyed importKeyedA = container.Locate<IImportKeyed>(withKey: 'A');

            Assert.NotNull(importKeyedA);
            Assert.IsType<SimpleObjectA>(importKeyedA.SimpleObject);

            Assert.Equal(3, container.LocateAll<IImportKeyed>(withKey: new[] { 'A', 'C', 'E' }).Count);
        }

        #endregion

        #region new context

        [Fact]
        public void InNewContextTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                      {
                                          c.Export<ContextSingleton>()
                                            .ByInterfaces()
                                            .UsingLifestyle(new SingletonPerInjectionContextLifestyle());
                                          c.Export<ContextClassA>().ByInterfaces();
                                          c.Export<ContextClassB>().ByInterfaces().InNewContext();
                                          c.Export<ContextClassC>().ByInterfaces();
                                      });

            IContextClassA classA = container.Locate<IContextClassA>();

            Assert.NotNull(classA);
            Assert.NotNull(classA.ContextClassB);
            Assert.NotNull(classA.ContextClassB.ContextClassC);

            Assert.NotNull(classA.ContextSingleton);
            Assert.NotNull(classA.ContextClassB.ContextSingleton);
            Assert.NotNull(classA.ContextClassB.ContextClassC.ContextSingleton);

            Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextSingleton);
            Assert.NotSame(classA.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);
            Assert.Same(classA.ContextClassB.ContextSingleton, classA.ContextClassB.ContextClassC.ContextSingleton);

        }

        #endregion

        #region BeginLifetimeScope

        [Fact]
        public void BeginLifetimeScope()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().AndSingletonPerScope());

            IDisposableService service = container.Locate<IDisposableService>();

            Assert.NotNull(service);

            bool called = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var secondService = scope.Locate<IDisposableService>();

                Assert.NotNull(secondService);
                Assert.NotSame(service, secondService);
                Assert.Same(secondService, scope.Locate<IDisposableService>());

                secondService.Disposing += (sender, args) => called = true;
            }

            Assert.True(called);
        }
        #endregion

        #region WithNamedCtorValue
        [Fact]
        public void WithNamedCtorValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export(typeof(DateTimeImport)).WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }

        [Fact]
        public void WithNamedCtorValueGeneric()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>().WithNamedCtorValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }


        [Fact]
        public void WithNamedCtorValueGenericNow()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<NowDateTimeImport>().WithNamedCtorValue(() => DateTime.Now));

            NowDateTimeImport import = container.Locate<NowDateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(import.CurrentTime.Date, DateTime.Now.Date);
        }

        [Fact]
        public void ExportNamedValue()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();
            DateTime currentTime = DateTime.Now;

            container.Configure(c => c.Export<DateTimeImport>());
            container.Configure(c => c.ExportNamedValue(() => currentTime));

            DateTimeImport import = container.Locate<DateTimeImport>();

            Assert.NotNull(import);
            Assert.Equal(currentTime, import.CurrentTime);
        }
        #endregion

        #region WithInspectorFor

        [Fact]
        public void WithInspectorFor()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .WithInspectorFor<ISimpleObject>(e => e.AddMetadata("SimpleObject", e.ActivationType)));

            var list =
                container.LocateAll<ISimpleObject>(consider: ExportsThat.HaveMetadata("SimpleObject"));

            Assert.Equal(5, list.Count);

            list =
                container.LocateAll<ISimpleObject>(consider:
                    ExportsThat.HaveMetadata("SimpleObject", typeof(SimpleObjectA)));

            Assert.Equal(1, list.Count);
        }



        #endregion

        #region BasedOn Interface Test

        [Fact]
        public void BasedOnInterfaceTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                              .BasedOn<ISimpleObject>()
                                              .ByInterfaces());

            IEnumerable<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count());
        }

        [Fact]
        public void BasedOnInterfaceByType()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                              .BasedOn<ISimpleObject>()
                                              .ByType());

            SimpleObjectA simpleObjectA = container.Locate<SimpleObjectA>();

            Assert.NotNull(simpleObjectA);
        }

        #endregion

        #region TypesThat tests

        [Fact]
        public void TypesThatAreBasedOn()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>()));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

            Assert.Equal(5, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void TypesThatAreBasedOnChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>().And.HaveAttribute<SimpleFilterAttribute>()));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count);

            Assert.Equal(3, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void TypesThatAreBasedOnComplexChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn(TypesThat.StartWith("ISimple"))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

            Assert.Equal(5, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void TypesThatHaveAttributeChainedFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.AreBasedOn(TypesThat.HaveAttribute(TypesThat.StartWith("Simple")))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

            Assert.Equal(5, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void FromThisAssemblyFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly(consider: TypesThat.AreBasedOn(TypesThat.HaveAttribute(TypesThat.StartWith("Simple")))))
                                      .ByInterfaces());

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(5, simpleObjects.Count);

            Assert.Equal(5, container.Diagnose().Exports.Count());
        }

        [Fact]
        public void TypesThatHaveAttributeFilter()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByInterfaces()
                                      .Select(TypesThat.HaveAttribute(TypesThat.StartWith("Simple"))));

            List<ISimpleObject> simpleObjects = container.LocateAll<ISimpleObject>();

            Assert.NotNull(simpleObjects);
            Assert.Equal(3, simpleObjects.Count);

            Assert.Equal(3, container.Diagnose().Exports.Count());
        }
        #endregion
    }
}
