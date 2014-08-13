using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when an injection scope by the specified name couldn't be found
    /// </summary>
    public class InjectionScopeCouldNotBeFoundException : Exception
    {
        public InjectionScopeCouldNotBeFoundException(string scopeName)
            : base("Could not locate scope named " + scopeName)
        {
            
        }
    }
}
