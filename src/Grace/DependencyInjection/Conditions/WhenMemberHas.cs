using System;
using System.Linq;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// Condition for if a member has an attribute
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    public class WhenMemberHas<TAttribute> : ICompiledCondition where TAttribute : Attribute
    {
        private readonly Func<TAttribute, bool> _filter;

        /// <summary>
        /// Default constructor that takes a filter
        /// </summary>
        /// <param name="filter"></param>
        public WhenMemberHas(Func<TAttribute, bool> filter)
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
            var targetInfo = staticInjectionContext.InjectionStack.FirstOrDefault(
                info => info.RequestingStrategy?.StrategyType == ActivationStrategyType.ExportStrategy);

            if (targetInfo != null)
            {
                foreach (var attribute in targetInfo.InjectionMemberAttributes)
                {
                    if (attribute is TAttribute attrT && (_filter == null || _filter(attrT)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
