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

        [Fact]
        public void Container_BeginLifetimeScope_Locate_ExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DependentService<IExportLocatorScope>>().Lifestyle.SingletonPerScope());

            using (var scope1 = container.BeginLifetimeScope())
            {
                using (var scope2 = container.BeginLifetimeScope())
                {
                    var instance1 = scope1.Locate<DependentService<IExportLocatorScope>>();
                    var instance2 = scope2.Locate<DependentService<IExportLocatorScope>>();

                    Assert.NotNull(instance1);
                    Assert.NotNull(instance2);
                    Assert.NotSame(instance1, instance2);
                }
            }
        }
    }
}
