using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// condition class for testing if target has a particular attribute
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    public class WhenClassHas<TAttribute> : ICompiledCondition where TAttribute : Attribute
    {
        private readonly Func<TAttribute, bool> _filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filter"></param>
        public WhenClassHas(Func<TAttribute, bool> filter)
        {
            _filter = filter;
        }
        
        /// <summary>
        /// Test if strategy meets condition
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            var targetInfo = staticInjectionContext.TargetInfo;

            if (targetInfo != null)
            {
                foreach (var attribute in targetInfo.InjectionTypeAttributes)
                {
                    var attrT = attribute as TAttribute;

                    if (attrT != null && (_filter == null || _filter(attrT)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
