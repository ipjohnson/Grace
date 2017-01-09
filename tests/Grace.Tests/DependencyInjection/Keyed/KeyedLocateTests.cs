using Grace.DependencyInjection;
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
        public void AsKeyedStringTest()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.ExportInstance((scope, context) => "Hello");
                c.ExportInstance((scope, context) => "HelloAgain").AsKeyed<string>("Key");
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
                Assert.True(simpleObject.GetType().FullName.EndsWith(locateChar.ToString()));
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
    }
}
