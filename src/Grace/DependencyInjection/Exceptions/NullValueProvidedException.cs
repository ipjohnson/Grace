using System;

namespace Grace.DependencyInjection.Exceptions
{
    public class NullValueProvidedException : LocateException
    {
        public NullValueProvidedException(StaticInjectionContext context) : base(context)
        {
        }

        public NullValueProvidedException(StaticInjectionContext context, Exception innerException) : base(context, innerException)
        {
        }
    }
}
