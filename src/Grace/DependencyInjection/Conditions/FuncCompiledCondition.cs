using System;

namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// Condition that calls a function to test if conditions are meet
    /// </summary>
    public class FuncCompiledCondition : ICompiledCondition
    {
        private readonly Func<IActivationStrategy, StaticInjectionContext, bool> _condition;

        /// <summary>
        /// Default constructor takes condition function
        /// </summary>
        /// <param name="condition">condition function</param>
        public FuncCompiledCondition(Func<IActivationStrategy, StaticInjectionContext, bool> condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            _condition = condition;
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
        /// Test if condition is meet
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <param name="context"></param>
        /// <returns>true if condition is meet</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext, IInjectionContext context)
        {
            return _condition(strategy, staticInjectionContext);
        }
    }
}
