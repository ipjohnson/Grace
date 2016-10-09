using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Exceptions
{
    public class RecursiveLocateException : LocateException
    {
        public RecursiveLocateException(StaticInjectionContext context) : base(context)
        {
        }

        public RecursiveLocateException(StaticInjectionContext context, Exception innerException) : base(context, innerException)
        {
        }
    }
}
