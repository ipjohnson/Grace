using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.LanguageExtensions;

namespace Grace.ExampleApp.DependencyInjection.LifetimeScopes
{
    public class LifetimeScopesSubModules : IExampleSubModule<DependencyInjectionExampleModule>
    {
        private readonly IReadOnlyCollection<IExample<LifetimeScopesSubModules>> _examples;

        public LifetimeScopesSubModules(IReadOnlyCollection<IExample<LifetimeScopesSubModules>> examples)
        {
            _examples = examples;
        }

        public void Execute()
        {
            _examples.Apply(x => x.ExecuteExample());
        }
    }
}
