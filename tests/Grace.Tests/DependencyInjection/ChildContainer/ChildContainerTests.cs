using System;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ChildContainer
{
    public class ChildContainerTests
    {
        [Fact]
        public void ChildContainer_Dispose_Transient_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>();
            });

            var disposedCalled = false;

            using (var childContainer = container.CreateChildScope())
            {
                var disposable = childContainer.Locate<IDisposableService>();

                Assert.NotNull(disposable);

                disposable.Disposing += (sender, args) => disposedCalled = true;
            }

            Assert.True(disposedCalled);
        }

        [Fact]
        public void ChildContainer_Dispose_Singleton_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>().Lifestyle.Singleton();
            });

            var disposedCalled = false;

            using (var childContainer = container.CreateChildScope())
            {
                var disposable = childContainer.Locate<IDisposableService>();

                Assert.NotNull(disposable);

                disposable.Disposing += (sender, args) => disposedCalled = true;
            }

            Assert.False(disposedCalled);

            container.Dispose();

            Assert.True(disposedCalled);
        }

        [Fact]
        public void ChildContainer_Locate_Dependency_From_Parent_Container()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var childContainer = container.CreateChildScope(c => c.Export<DependentService<IBasicService>>().As<IDependentService<IBasicService>>()))
            {
                var instance = childContainer.Locate<IDependentService<IBasicService>>();

                Assert.NotNull(instance);
            }

            Assert.ThrowsAny<Exception>(() => container.Locate<IDependentService<IBasicService>>());
        }

        [Fact]
        public void ChildContainer_Locate_Correct_Instance()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>());

            using (var child = container.CreateChildScope())
            {
                child.Configure(c =>
                {
                    c.ExportFactory<IBasicService>(() => new BasicService { Count = 10 });
                    c.ExportFactory<IExportLocatorScope, DependentService<IBasicService>>(scope => new DependentService<IBasicService>(scope.Locate<IBasicService>()));
                });

                var instance = child.Locate<DependentService<IBasicService>>();

                Assert.NotNull(instance);
                Assert.Equal(10, instance.Value.Count);
            }
        }
    }
}
