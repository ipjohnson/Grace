using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.UnitTests.Classes.Simple
{
    public class ImportInjectionScopeClass
    {
        public ImportInjectionScopeClass()
        {

        }

        public ImportInjectionScopeClass(IInjectionScope scope)
        {
            Scope = scope;
        }

        public IInjectionScope Scope { get; }
    }
}
