using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    public class NamedScopeLocateException : LocateException
    {
        public NamedScopeLocateException(string scopeName, StaticInjectionContext context) : base(context)
        {
            ScopeName = scopeName;
        }
        
        public string ScopeName { get; }
    }
}
