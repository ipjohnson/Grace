using System;
using Grace.DependencyInjection.Impl.FactoryStrategies;

namespace Grace.DependencyInjection.Impl.KnownTypeStrategies
{
    /// <summary>
    /// Strategy for creating Func&lt;Type,object&gt;
    /// </summary>
    public class FuncTypeStrategy : FactoryTwoArgStrategy<IExportLocatorScope,IInjectionContext,Func<Type,object>>
    {
        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public FuncTypeStrategy(IInjectionScope injectionScope) : base(CreateFunc, injectionScope)
        {
        }

        private static Func<Type, object> CreateFunc(IExportLocatorScope scope, IInjectionContext context)
        {
            return type =>
            {
                var clone = context.Clone();

                return scope.Locate(type, clone);
            };
        }
    }
}
