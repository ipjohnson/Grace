using System;
using System.Linq;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// condition for when target has attribute
    /// </summary>
    public class WhenTargetHas : ICompiledCondition
    {
        private readonly Type _attributeType;
        private readonly Func<Attribute, bool> _filter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="filter"></param>
        public WhenTargetHas(Type attributeType, Func<Attribute,bool> filter = null)
        {
            _attributeType = attributeType;
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
                foreach (var attribute in targetInfo.InjectionTargetAttributes)
                {
                    if (attribute.GetType() == _attributeType && (_filter == null || _filter(attribute)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
