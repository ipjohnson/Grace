using System;

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
                foreach (var attribute in targetInfo.InjectionMemberAttributes)
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
