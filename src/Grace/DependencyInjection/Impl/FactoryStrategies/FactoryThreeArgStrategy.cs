using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 3 dependencies and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryThreeArgStrategy<T1, T2, T3, TResult> : DelegateBaseExportStrategy<Func<T1, T2, T3, TResult>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func">delegate</param>
        /// <param name="injectionScope">injection scope</param>
        public FactoryThreeArgStrategy(Func<T1, T2, T3, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope, func)
        {
        }
    }
}
