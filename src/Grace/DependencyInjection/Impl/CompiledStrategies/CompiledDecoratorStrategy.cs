using System;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Strategy for user registered decorators
    /// </summary>
    public class CompiledDecoratorStrategy : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private readonly IDefaultStrategyExpressionBuilder _builder;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="builder"></param>
        public CompiledDecoratorStrategy(Type activationType, IInjectionScope injectionScope, IDefaultStrategyExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        public bool ApplyAfterLifestyle { get; set; }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.DecoratorStrategy;

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return _builder.GetActivationExpression(scope, request, ActivationConfiguration, lifestyle);
        }
    }
}
