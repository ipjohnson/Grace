using System;
using Grace.DependencyInjection.Impl.InstanceStrategies;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 4 dependencies and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryFourArgStrategy<T1, T2, T3, T4, TResult> : DelegateBaseExportStrategy<Func<T1, T2, T3, T4, TResult>>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryFourArgStrategy(Func<T1, T2, T3, T4, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope, func)
        {

        }
    }
}
