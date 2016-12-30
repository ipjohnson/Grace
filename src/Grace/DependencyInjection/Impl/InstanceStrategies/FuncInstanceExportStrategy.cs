using System;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy that represents Func with no arguemnts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncInstanceExportStrategy<T> : DelegateBaseExportStrategy<Func<T>>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncInstanceExportStrategy(Func<T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope, func)
        {

        }
    }
}
