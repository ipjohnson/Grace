using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.LocateAll
{
    public class LocateAllTests
    {
        [Fact]
        public void LocateAll_Basic_Tests()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().As<IMultipleService>();
                c.Export<MultipleService2>().As<IMultipleService>();
                c.Export<MultipleService3>().As<IMultipleService>();
                c.Export<MultipleService4>().As<IMultipleService>();
                c.Export<MultipleService5>().As<IMultipleService>();
            });

            var objects = container.LocateAll(typeof(IMultipleService));

            var array = objects.OfType<IMultipleService>().ToArray();

            Assert.Equal(5, array.Length);
            Assert.IsType<MultipleService1>(array[0]);
            Assert.IsType<MultipleService2>(array[1]);
            Assert.IsType<MultipleService3>(array[2]);
            Assert.IsType<MultipleService4>(array[3]);
            Assert.IsType<MultipleService5>(array[4]);
        }

        [Fact]
        public void LocateAll_Filter()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<SimpleObjectA>().As<ISimpleObject>();
                c.Export<SimpleObjectB>().As<ISimpleObject>();
                c.Export<SimpleObjectC>().As<ISimpleObject>();
                c.Export<SimpleObjectD>().As<ISimpleObject>();
                c.Export<SimpleObjectE>().As<ISimpleObject>();
            });

            var list = container.LocateAll(typeof(ISimpleObject), 
                consider: s => s.ActivationType.GetTypeInfo().GetCustomAttributes().Any(a => a is SimpleFilterAttribute));

            var array = list.OfType<ISimpleObject>().ToArray();

            Assert.Equal(3, array.Length);

            Assert.IsType<SimpleObjectA>(array[0]);
            Assert.IsType<SimpleObjectC>(array[1]);
            Assert.IsType<SimpleObjectE>(array[2]);
        }
    }
}
