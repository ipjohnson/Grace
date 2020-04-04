using System;
using Grace.DependencyInjection.Conditions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// configuration object for decorator
    /// </summary>
    public class FluentDecoratorStrategyConfiguration : IFluentDecoratorStrategyConfiguration, IFluentDecoratorFactoryStrategyConfiguration
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

        /// <inheritdoc />
        public IFluentDecoratorStrategyConfiguration Priority(int priority)
        {
            _strategy.Priority = priority;

            return this;
        }

        /// <summary>
        /// Add a specific value for a particuar parameter in the constructor
        /// </summary>
        /// <typeparam name="TParam">type of parameter</typeparam>
        /// <param name="paramValue">Func(T) value for the parameter</param>
        /// <returns>configuration object</returns>
        public IFluentDecoratorWithCtorConfiguration<TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
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
            if (paramValue == null) throw new ArgumentNullException(nameof(paramValue));

            var parameterInfo = new ConstructorParameterInfo(paramValue) { ParameterType = typeof(TParam) };

            _strategy.ConstructorParameter(parameterInfo);

            return new FluentDecoratorWithCtorConfiguration<TParam>(this, parameterInfo);
        }
    }
}
