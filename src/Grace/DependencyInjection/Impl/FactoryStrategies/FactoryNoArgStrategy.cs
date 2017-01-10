using System;
using Grace.DependencyInjection.Impl.InstanceStrategies;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take no dependencies and returns TResult
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryNoArgStrategy<TResult> : DelegateBaseExportStrategy<Func<TResult>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryNoArgStrategy(Func<TResult> func , IInjectionScope injectionScope) : base(typeof(TResult), injectionScope, func)
        {
        }
    }
}
