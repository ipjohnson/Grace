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
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }
        
        /// <summary>
        /// Test if condition is meet
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>true if condition is meet</returns>
        public bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext)
        {
            return _condition(strategy, staticInjectionContext);
        }
    }
}
