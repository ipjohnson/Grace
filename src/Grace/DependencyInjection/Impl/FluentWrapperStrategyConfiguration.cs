using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Configuration object for wrapper strategy
    /// </summary>
    public class FluentWrapperStrategyConfiguration : IFluentWrapperStrategyConfiguration
    {
        private readonly IConfigurableCompiledWrapperStrategy _compiledWrapperStrategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="compiledWrapperStrategy"></param>
        public FluentWrapperStrategyConfiguration(IConfigurableCompiledWrapperStrategy compiledWrapperStrategy)
        {
            _compiledWrapperStrategy = compiledWrapperStrategy;
        }

        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWrapperStrategyConfiguration As(Type type)
        {
            _compiledWrapperStrategy.AddExportAs(type);

            return this;
        }

        /// <summary>
        /// Apply a condition on when to use strategy
        /// </summary>
        public IWhenConditionConfiguration<IFluentWrapperStrategyConfiguration> When =>
            new WhenConditionConfiguration<IFluentWrapperStrategyConfiguration>(c => _compiledWrapperStrategy.AddCondition(c), this);

        /// <summary>
        /// Set the type that is being wrapped
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentWrapperStrategyConfiguration WrappedType(Type type)
        {
            _compiledWrapperStrategy.SetWrappedType(type);

            return this;
        }

        /// <summary>
        /// set the position of the generic arg being wrapped
        /// </summary>
        /// <param name="genericArgPosition"></param>
        /// <returns></returns>
        public IFluentWrapperStrategyConfiguration WrappedGenericArg(int genericArgPosition)
        {
            _compiledWrapperStrategy.SetWrappedGenericArgPosition(genericArgPosition);

            return this;
        }
    }
}
