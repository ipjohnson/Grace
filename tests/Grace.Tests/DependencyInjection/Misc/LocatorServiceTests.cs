using Grace.DependencyInjection;
using Xunit;

namespace Grace.Tests.DependencyInjection.Misc
{
    public class LocatorServiceTests
    {
        [Fact]
        public void DependencyInjectionContainer_Locate_ILocatorService()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                Assert.Same(scope, scope.Locate<ILocatorService>());
            }
        }

        public class ImportILocatorService
        {
            public ImportILocatorService(ILocatorService locatorService)
            {
                LocatorService = locatorService;
            }

            public ILocatorService LocatorService { get; }
        }

        [Fact]
        public void Import_ILocatorService()
        {
            var container = new DependencyInjectionContainer();

            var instance = container.Locate<ImportILocatorService>();

            Assert.Same(container,instance.LocatorService);
        }

        [Fact]
        public void Import_LifetimeScope_ILocatorService()
        {
            var container = new DependencyInjectionContainer();

            using (var scope = container.BeginLifetimeScope())
            {
                var instance = scope.Locate<ImportILocatorService>();

                Assert.Same(scope, instance.LocatorService);
            }
        }
    }
}
