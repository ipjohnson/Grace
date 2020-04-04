using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configuration interface for decorator strategy
    /// </summary>
    public interface IFluentDecoratorStrategyConfiguration
    {
        /// <summary>
        /// Apply decorator after lifestyle, by default it's before
        /// </summary>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration ApplyAfterLifestyle();

        /// <summary>
        /// Export as particular types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration As(Type type);

        /// <summary>
        /// Condition for decorator strategy
        /// </summary>
        IWhenConditionConfiguration<IFluentDecoratorStrategyConfiguration> When { get; }

        /// <summary>
        /// Sets priority for decorator
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IFluentDecoratorStrategyConfiguration Priority(int priority);

        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null);
        
        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TParam>(Func<TArg1, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TParam>(Func<TArg1, TArg2, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TParam>(Func<TArg1, TArg2, TArg3, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TParam> paramValue);

        /// <summary>
        /// Add a specific value for a particular parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TArg5, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TParam> paramValue);
    }
}
