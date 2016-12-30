using System;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for Func that takes IExportLocatorScope and StaticInjectionContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncWithStaticContextInstanceExportStrategy<T> : DelegateBaseExportStrategy<Func<IExportLocatorScope, StaticInjectionContext, T>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncWithStaticContextInstanceExportStrategy(Func<IExportLocatorScope,StaticInjectionContext,T> func, IInjectionScope injectionScope) : 
            base(typeof(T), injectionScope, func)
        {

        }
    }
}
