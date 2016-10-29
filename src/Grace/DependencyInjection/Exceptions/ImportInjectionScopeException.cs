using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when trying to import IInjectionScope. Under most circumstances you want to inject IExportLocatorScope
    /// </summary>
    public class ImportInjectionScopeException : LocateException
    {
        public const string ErrorMessage = "IInjectionScope was requested, please change it to IExportLocatorScope.";

        public ImportInjectionScopeException(StaticInjectionContext context) : base(context, ErrorMessage)
        {

        }
    }
}
