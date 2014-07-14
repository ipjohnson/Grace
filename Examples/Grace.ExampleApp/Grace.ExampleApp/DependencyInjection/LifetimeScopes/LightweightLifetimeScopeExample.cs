using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.ExampleApp.DependencyInjection.ExampleClasses;

namespace Grace.ExampleApp.DependencyInjection.LifetimeScopes
{
    public class LightweightLifetimeScopeExample : IExample<LifetimeScopesSubModules>
    {
        public void ExecuteExample()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<BasicService>().As<IBasicService>().Lifestyle.SingletonPerScope());

            IBasicService basicService = container.Locate<IBasicService>();
            IBasicService basicService2 = container.Locate<IBasicService>();

            if (basicService != basicService2)
            {
                throw new Exception("Should be the same service");
            }

            using (var scope = container.BeginLifetimeScope())
            {
                IBasicService nestedService = scope.Locate<IBasicService>();
                IBasicService nestedService2 = scope.Locate<IBasicService>();

                if (nestedService != nestedService2)
                {
                    throw new Exception("Should be same serice in the same scope");
                }

                if (nestedService == basicService)
                {
                    throw new Exception("Services should not be the same");
                }
            }
        }
    }

    public class DisposedLightweightScopeExample : IExample<LifetimeScopesSubModules>
    {
        public void ExecuteExample()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<DisposedService>().As<IDisposedService>());

            IDisposedService disposedService = container.Locate<IDisposedService>();

            bool disposed1 = false;
            bool disposed2 = false;
            disposedService.Disposed += (sender, args) => disposed1 = true;

            using (var scope = container.BeginLifetimeScope())
            {
                IDisposedService disposedService2 = scope.Locate<IDisposedService>();

                disposedService2.Disposed += (sender, args) => disposed2 = true;
            }

            if (!disposed2)
            {
                throw new Exception("Should have disposed service 2");
            }

            if (disposed1)
            {
                throw new Exception("Should not have disposed yet");
            }

            container.Dispose();

            if (!disposed1)
            {
                throw new Exception("Should have disposed already");
            }
        }
    }
}
