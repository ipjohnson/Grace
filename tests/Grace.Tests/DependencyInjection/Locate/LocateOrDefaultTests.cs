using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.Locate
{
    public class LocateOrDefaultTests
    {
        [Fact]
        public void LocateOrDefault_Generic_Return_Export()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var basicService = new BasicService();

            var instance = container.LocateOrDefault<IBasicService>(basicService);

            Assert.NotNull(instance);
            Assert.NotSame(basicService, instance);
        }
        
        [Fact]
        public void LocateOrDefault_Generic_Return_Default()
        {
            var container = new DependencyInjectionContainer();
            
            var basicService = new BasicService();

            var instance = container.LocateOrDefault<IBasicService>(basicService);

            Assert.NotNull(instance);
            Assert.Same(basicService, instance);
        }
        
        [Fact]
        public void LocateOrDefault_NonGeneric_Return_Export()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            var basicService = new BasicService();

            var instance = container.LocateOrDefault(typeof(IBasicService), basicService);

            Assert.NotNull(instance);
            Assert.NotSame(basicService, instance);
        }

        [Fact]
        public void LocateOrDefault_NonGeneric_Return_Default()
        {
            var container = new DependencyInjectionContainer();
            
            var basicService = new BasicService();

            var instance = container.LocateOrDefault(typeof(IBasicService),basicService);

            Assert.NotNull(instance);
            Assert.Same(basicService, instance);
        }
    }
}
