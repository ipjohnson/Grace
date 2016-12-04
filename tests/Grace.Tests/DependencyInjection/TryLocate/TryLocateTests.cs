using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.TryLocate
{
    public class TryLocateTests
    {
        [Fact]
        public void TryLocate_Interface()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            IBasicService service;

            container.TryLocate(out service);

            Assert.Null(service);
        }

        [Fact]
        public void TryLocate_Missing_Interface()
        {
            var container = new DependencyInjectionContainer();

            IBasicService service;

            container.TryLocate(out service);

            Assert.Null(service);
        }
    }
}
