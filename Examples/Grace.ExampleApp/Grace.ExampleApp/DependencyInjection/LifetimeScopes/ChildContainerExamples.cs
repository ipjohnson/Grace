using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.LifetimeScopes
{
    public class BasicChildContainerExample : IExample<LifetimeScopesSubModules>
    {
        public void ExecuteExample()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<ImportConstructor>().As<IImportConstructor>());

            using (var childContainer = container.CreateChildScope(c => c.Export<BasicService>().As<IBasicService>()))
            {
                IImportConstructor constructor = childContainer.Locate<IImportConstructor>();

                if (constructor == null)
                {
                    throw new Exception("Could not construct IImportConstructor");
                }
            }
        }
    }

    public class DisposableChildContainerExample : IExample<LifetimeScopesSubModules>
    {
        public void ExecuteExample()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposedService>().As<IDisposedService>());

            IDisposedService disposedService = container.Locate<IDisposedService>();

            bool firstInstanceDisposed = false;

            disposedService.Disposed += (o,e) => firstInstanceDisposed = true;

            bool secondInstanceDisposed = false;

            using (var childContainer = container.CreateChildScope())
            {
                IDisposedService secondService = childContainer.Locate<IDisposedService>();

                secondService.Disposed += (sender, args) => secondInstanceDisposed = true;
            }

            if (!secondInstanceDisposed)
            {
                throw new Exception("Second instance should have disposed");
            }

            if (firstInstanceDisposed)
            {
                throw new Exception("Should not have disposed first instance");
            }

            container.Dispose();

            if (!firstInstanceDisposed)
            {
                throw new Exception("First instance should have been disposed here");
            }
        }
    }
}
