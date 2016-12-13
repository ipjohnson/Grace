using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Misc
{
    public class ExportLocatorScopeTests
    {
        [Fact]
        public void DependencyInjectionContainer_Locate_IExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.Same(scope, scope.Locate<IExportLocatorScope>());
            }
        }

        public class ImportExportLocatorScope
        {
            public ImportExportLocatorScope(IExportLocatorScope scope)
            {
                Scope = scope;
            }

            public IExportLocatorScope Scope { get; }
        }

        [Fact]
        public void Import_IExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            var instance = container.Locate<ImportExportLocatorScope>();

            Assert.Same(container, instance.Scope);
        }
        
        [Fact]
        public void Import_LifetimeScope_IExportLocatorScope()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<ImportExportLocatorScope>();

                Assert.Same(scope, instance.Scope);
            }
        }
    }
}
