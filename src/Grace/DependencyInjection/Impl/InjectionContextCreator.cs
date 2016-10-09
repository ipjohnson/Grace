using System;

namespace Grace.DependencyInjection.Impl
{
    public class InjectionContextCreator : IInjectionContextCreator
    {
        public IInjectionContext CreateContext(Type type, object extraData)
        {
            return new InjectionContext(extraData);
        }
    }
}
