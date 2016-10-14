using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.DisposableTests
{
    public class DisposalCleanupTests
    {
        [Fact]
        public void Export_DisposableCleanup_Called()
        {
            var container = new DependencyInjectionContainer();

            bool disposedDelegateCalled = false;

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
