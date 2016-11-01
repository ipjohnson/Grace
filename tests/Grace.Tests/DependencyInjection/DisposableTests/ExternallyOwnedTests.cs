using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.DisposableTests
{
    public class ExternallyOwnedTests
    {
        [Fact]
        public void ExternallyOwned_ExportStrategy_Not_Disposed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposableService>().As<IDisposableService>().ExternallyOwned());

            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<IDisposableService>();

                instance.Disposing += (sender, args) => disposed = true;
            }

            Assert.False(disposed);
        }

        [Fact]
        public void ExternallyOwned_ExportInstance_Not_Disposed()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.ExportFactory(() => new DisposableService()).As<IDisposableService>().ExternallyOwned());

            var disposed = false;

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<IDisposableService>();

                instance.Disposing += (sender, args) => disposed = true;
            }

            Assert.False(disposed);
        }
    }
}
