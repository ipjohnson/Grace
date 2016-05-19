using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    /// <summary>
    /// Cannot locate export 
    /// </summary>
    public class CannotLocateExportException : LocateException
    {
        public CannotLocateExportException(string locateName, Type locatingType, IInjectionContext currentContext) :
            base(locateName, locatingType, currentContext)
        {
            AddLocationInformationEntry(new LocationInformationEntry(locateName, locatingType, currentContext.TargetInfo));
        }
    }
}
