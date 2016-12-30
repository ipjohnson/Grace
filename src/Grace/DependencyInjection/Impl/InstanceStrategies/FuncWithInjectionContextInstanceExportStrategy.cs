using System;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for a Func that takes locator scope, static injection context, and IInjectionContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncWithInjectionContextInstanceExportStrategy<T> : DelegateBaseExportStrategy<Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncWithInjectionContextInstanceExportStrategy(
            Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> func, IInjectionScope injectionScope)
            :
            base(typeof(T), injectionScope, func)
        {

        }
    }
}
