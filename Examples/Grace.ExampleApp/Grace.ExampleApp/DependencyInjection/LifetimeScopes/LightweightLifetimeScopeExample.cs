using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.ExampleApp.DependencyInjection.LifetimeScopes
{
    public class LightweightLifetimeScopeExample : IExample<LifetimeScopesSubModules>
    {
        public void ExecuteExample()
        {
            DependencyInjectionContainer container = new DependencyInjectionContainer();

            
        }
    }
}
