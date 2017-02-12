using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// configuration object for decorator
    /// </summary>
    public class FluentDecoratorStrategyConfiguration : IFluentDecoratorStrategyConfiguration
    {
        private readonly ICompiledDecoratorStrategy _strategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strategy"></param>
        public FluentDecoratorStrategyConfiguration(ICompiledDecoratorStrategy strategy)
        {
            _strategy = strategy;
        }


        /// <summary>
        /// Apply decorator after lifestyle, by default it's before
        /// </summary>
        /// <returns></returns>
        public IFluentDecoratorStrategyConfiguration ApplyAfterLifestyle()
        {
            _strategy.ApplyAfterLifestyle = true;

            return this;
        }

        /// <summary>
        /// Export as particular types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentDecoratorStrategyConfiguration As(Type type)
        {
            _strategy.AddExportAs(type);

            return this;
        }

        /// <summary>
        /// Condition for decorator strategy
        /// </summary>
        public IWhenConditionConfiguration<IFluentDecoratorStrategyConfiguration> When 
            => new WhenConditionConfiguration<IFluentDecoratorStrategyConfiguration>(_strategy.AddCondition, this);
    }
}
