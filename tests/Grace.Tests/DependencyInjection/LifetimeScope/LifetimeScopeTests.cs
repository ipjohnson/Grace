using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.LifetimeScope
{
    public class LifetimeScopeTests
    {
        [Fact]
        public void Container_BeingLifetimeScope_SimpleResolve()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<BasicService>().As<IBasicService>();
            });

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var basicService = lifetimeScope.Locate<IBasicService>();

                Assert.NotNull(basicService);
                Assert.IsType<BasicService>(basicService);
            }
        }

        [Fact]
        public void Container_BeginLifetimeScope_DisposeCorrectly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>());

            var disposedCalled = false;

            using (var lifetimeScope = container.BeginLifetimeScope())
            {
                var disposed = lifetimeScope.Locate<IDisposableService>();

                Assert.NotNull(disposed);

                disposed.Disposing += (sender, args) => disposedCalled = true;
            }

            Assert.True(disposedCalled);
        }
    }
}
