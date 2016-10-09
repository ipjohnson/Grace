using System;

namespace Grace.DependencyInjection.Impl
{
    public interface IInjectionContextCreator
    {
        IInjectionContext CreateContext(Type type, object extraData);
    }
}
