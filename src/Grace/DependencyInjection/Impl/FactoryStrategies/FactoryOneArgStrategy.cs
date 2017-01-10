using System;
using Grace.DependencyInjection.Impl.InstanceStrategies;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 1 dependency and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryOneArgStrategy<T1, TResult> : DelegateBaseExportStrategy<Func<T1, TResult>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryOneArgStrategy(Func<T1, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope, func)
        {
        }
    }
}
