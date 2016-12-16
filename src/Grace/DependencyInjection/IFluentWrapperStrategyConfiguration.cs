using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configuration object for wrapper strategy
    /// </summary>
    public interface IFluentWrapperStrategyConfiguration
    {
        /// <summary>
        /// Export as a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWrapperStrategyConfiguration As(Type type);


        /// <summary>
        /// Apply a condition on when to use strategy
        /// </summary>
        IWhenConditionConfiguration<IFluentWrapperStrategyConfiguration> When { get; }

        /// <summary>
        /// Set the type that is being wrapped
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentWrapperStrategyConfiguration WrappedType(Type type);

        /// <summary>
        /// set the position of the generic arg being wrapped
        /// </summary>
        /// <param name="genericArgPosition"></param>
        /// <returns></returns>
        IFluentWrapperStrategyConfiguration WrappedGenericArg(int genericArgPosition);
    }
}
