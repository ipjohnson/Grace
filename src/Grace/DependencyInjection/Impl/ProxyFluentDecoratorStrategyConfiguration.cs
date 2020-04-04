using System;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyFluentDecoratorStrategyConfiguration : IFluentDecoratorStrategyConfiguration
    {
        private readonly IFluentDecoratorStrategyConfiguration _configuration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ProxyFluentDecoratorStrategyConfiguration(IFluentDecoratorStrategyConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Apply decorator after lifestyle, by default it's before
        /// </summary>
        /// <returns></returns>
        public IFluentDecoratorStrategyConfiguration ApplyAfterLifestyle()
        {
            return _configuration.ApplyAfterLifestyle();
        }

        /// <summary>
        /// Export as particular types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFluentDecoratorStrategyConfiguration As(Type type)
        {
            return _configuration.As(type);
        }

        /// <summary>
        /// Condition for decorator strategy
        /// </summary>
        public IWhenConditionConfiguration<IFluentDecoratorStrategyConfiguration> When => _configuration.When;


        /// <inheritdoc />
        public IFluentDecoratorStrategyConfiguration Priority(int priority)
        {
            return _configuration.Priority(priority);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            return _configuration.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TParam>(Func<TArg1, TParam> paramValue)
        {
            return _configuration.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TParam>(Func<TArg1, TArg2, TParam> paramValue)
        {
            return _configuration.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TParam>(Func<TArg1, TArg2, TArg3, TParam> paramValue)
        {
            return _configuration.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TParam> paramValue)
        {
            return _configuration.WithCtorParam(paramValue);
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="paramValue">Func(IInjectionScope, IInjectionContext, T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TArg1, TArg2, TArg3, TArg4, TArg5, TParam>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TParam> paramValue)
        {
            return _configuration.WithCtorParam(paramValue);
        }
    }
}
