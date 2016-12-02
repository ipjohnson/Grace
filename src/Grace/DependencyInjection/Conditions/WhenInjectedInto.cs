using System;
using Grace.Data;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// Condition for testing if a strategy is being injected into another
    /// </summary>
    public class WhenInjectedInto : ICompiledCondition
    {
        private readonly Type[] _types;

        /// <summary>
        /// Default constructor takes list of types
        /// </summary>
        /// <param name="types"></param>
        public WhenInjectedInto(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            _types = types;
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
        /// Test if being injected into a specific type
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext, IInjectionContext context)
        {
            var targetInfo = staticInjectionContext.TargetInfo;

            if (targetInfo != null)
            {
                foreach (var type in _types)
                {
                    if (ReflectionService.CheckTypeIsBasedOnAnotherType(targetInfo.InjectionType, type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
