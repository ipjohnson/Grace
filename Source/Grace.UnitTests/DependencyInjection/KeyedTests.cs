using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Diagnostics;
using Grace.UnitTests.Classes.Simple;
using Xunit;

namespace Grace.UnitTests.DependencyInjection
{
    public class KeyedTests
    {

        #region WithKey tests

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

        #region AsKeyed

        [Fact]
        public void AsKeyedBasicTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c =>
                                {
                                    c.Export<SimpleObjectA>().AsKeyed<ISimpleObject, char>('A');
                                    c.Export<SimpleObjectB>().AsKeyed<ISimpleObject, char>('B');
                                    c.Export<SimpleObjectC>().AsKeyed<ISimpleObject, char>('C');
                                    c.Export<SimpleObjectD>().AsKeyed<ISimpleObject, char>('D');
                                    c.Export<SimpleObjectE>().AsKeyed<ISimpleObject, char>('E');
                                });

            for (char locateChar = 'A'; locateChar < 'F'; locateChar++)
            {
                var simpleObject = container.Locate<ISimpleObject>(withKey: locateChar);

                Assert.NotNull(simpleObject);
                Assert.True(simpleObject.GetType().FullName.EndsWith(locateChar.ToString()));
            }
        }

        [Fact]
        public void ByKeyedTypesTest()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export(Types.FromThisAssembly())
                                      .ByKeyedTypes(t => new[] { new Tuple<Type, object>(typeof(ISimpleObject), t.Name.Last()) })
                                      .Select(TypesThat.AreBasedOn<ISimpleObject>()));

            for (char locateChar = 'A'; locateChar < 'F'; locateChar++)
            {
                var simpleObject = container.Locate<ISimpleObject>(withKey: locateChar);

                Assert.NotNull(simpleObject);
                Assert.True(simpleObject.GetType().FullName.EndsWith(locateChar.ToString()));
            }
        }

        #endregion


    }
}
