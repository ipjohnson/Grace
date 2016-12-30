using System;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for Func that takes IExportLocatorScope
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncWithScopeInstanceExportStrategy<T> : DelegateBaseExportStrategy<Func<IExportLocatorScope, T>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncWithScopeInstanceExportStrategy(Func<IExportLocatorScope, T> func, IInjectionScope injectionScope) : 
            base(typeof(T), injectionScope, func)
        {

        }
    }
}
