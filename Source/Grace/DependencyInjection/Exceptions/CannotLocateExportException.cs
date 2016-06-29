using Grace.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Exception is thrown when an export can't be found
    /// </summary>
    public class CannotLocateExportException : LocateException
    {
        public CannotLocateExportException(string locateName, Type locatingType, IInjectionContext currentContext) :
            base(locateName, locatingType, currentContext)
        {

        }        
    }
}
