using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Creates new injection contexts
    /// </summary>
    public class InjectionContextCreator : IInjectionContextCreator
    {
        public IInjectionContext CreateContext(Type type, object extraData)
        {
            return extraData as IInjectionContext ?? new InjectionContext(extraData);
        }
    }
}
