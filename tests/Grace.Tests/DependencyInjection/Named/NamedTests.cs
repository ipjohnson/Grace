using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Named
{
    public class NamedTests
    {
        [Fact]
        public void Export_AsName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().AsName("BasicService"));

            var instance = container.LocateByName("BasicService");

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void Export_TypeSet_ByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportAssemblyContaining<NamedTests>().ByName());

            var instance = container.LocateByName("BasicService");

            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void TryLocateByName_Find()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().AsName("BasicService"));

            object instance;

            var returnValue = container.TryLocateByName("BasicService", out instance);

            Assert.True(returnValue);
            Assert.NotNull(instance);
            Assert.IsType<BasicService>(instance);
        }

        [Fact]
        public void TryLocateByName_Cant_Find()
        {
            var container = new DependencyInjectionContainer();
            
            object instance;

            var returnValue = container.TryLocateByName("BasicService", out instance);

            Assert.False(returnValue);
            Assert.Null(instance);
        }

        [Fact]
        public void LocateAllByName()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<MultipleService1>().AsName("MultipleService");
                c.Export<MultipleService2>().AsName("MultipleService");
                c.Export<MultipleService3>().AsName("MultipleService");
                c.Export<MultipleService4>().AsName("MultipleService");
                c.Export<MultipleService5>().AsName("MultipleService");
            });

            var services = container.LocateAllByName("MultipleService");
            
            Assert.Equal(5, services.Count);
            Assert.IsType<MultipleService1>(services[0]);
            Assert.IsType<MultipleService2>(services[1]);
            Assert.IsType<MultipleService3>(services[2]);
            Assert.IsType<MultipleService4>(services[3]);
            Assert.IsType<MultipleService5>(services[4]);
        }
    }
}
