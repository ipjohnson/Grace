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

            container.TryLocate(out IBasicService service);

            Assert.NotNull(service);
        }

        [Fact]
        public void TryLocate_Missing_Interface()
        {
            var container = new DependencyInjectionContainer();

            container.TryLocate(out IBasicService service);

            Assert.Null(service);
        }
    }
}
