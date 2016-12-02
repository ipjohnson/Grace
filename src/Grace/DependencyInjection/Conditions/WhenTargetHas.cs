using System;

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
        /// Should the condition be run at expression creation time or every time a request is made for the type
        /// </summary>
        public bool IsRequestTimeCondition { get; } = false;

        /// <summary>
        /// If it is a request time condition does it need an injection context
        /// </summary>
        public bool RequiresInjectionContext { get; } = false;

        /// <summary>
        /// Test if strategy meets condition
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <param name="context"></param>
        /// <returns>meets condition</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext, IInjectionContext context)
        {
            var targetInfo = staticInjectionContext.TargetInfo;

            if (targetInfo != null)
            {
                foreach (var attribute in targetInfo.InjectionTargetAttributes)
                {
                    if (attribute.GetType() != _attributeType && _filter == null || _filter(attribute))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
