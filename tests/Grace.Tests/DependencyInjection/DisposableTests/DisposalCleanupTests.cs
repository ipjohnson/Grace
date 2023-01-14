using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;
#if NET6_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Grace.Tests.DependencyInjection.DisposableTests
{

    public class DisposalCleanupTests
    {
#if NET6_0_OR_GREATER
        [Fact]
        public async Task Export_AsyncDisposableCleanup_Called()
        {
            var container = new DependencyInjectionContainer();

            var disposedDelegateCalled = false;

            container.Configure(c => c.Export<AsyncDisposableService>().As<IDisposableService>().DisposalCleanupDelegate(d => disposedDelegateCalled = true));

            await using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<IDisposableService>();

                Assert.NotNull(instance);
            }

            Assert.True(disposedDelegateCalled);
        }
#endif

        [Fact]
        public void Export_DisposableCleanup_Called()
        {
            var container = new DependencyInjectionContainer();

            var disposedDelegateCalled = false;

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().DisposalCleanupDelegate(d => disposedDelegateCalled = true));

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<IDisposableService>();

                Assert.NotNull(instance);
            }

            Assert.True(disposedDelegateCalled);
        }
    }
}
