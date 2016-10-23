using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Tests.Classes.Simple;
using Xunit;

namespace Grace.Tests.DependencyInjection.ChildContainer
{
    public class ChildContainerTests
    {
        [Fact]
        public void ChildContainer_Disposed_Correctly()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c =>
            {
                c.Export<DisposableService>().As<IDisposableService>();
            });

            bool disposedCalled = false;

            using (var childContianer = container.CreateChildScope())
            {
                var disposable = childContianer.Locate<IDisposableService>();

                Assert.NotNull(disposable);

                disposable.Disposing += (sender, args) => disposedCalled = true;
            }

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
    }
}
