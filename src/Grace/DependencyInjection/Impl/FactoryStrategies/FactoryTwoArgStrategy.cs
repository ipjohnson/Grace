using System;
using Grace.DependencyInjection.Impl.InstanceStrategies;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 2 dependencies and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryTwoArgStrategy<T1, T2, TResult> : DelegateBaseExportStrategy<Func<T1, T2, TResult>>
    {
        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryTwoArgStrategy(Func<T1, T2, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope, func)
        {
        }
    }
}
