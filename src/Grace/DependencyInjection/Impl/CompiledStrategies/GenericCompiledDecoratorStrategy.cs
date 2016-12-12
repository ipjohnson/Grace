using System;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Represents a generic decorator class
    /// </summary>
    public class GenericCompiledDecoratorStrategy : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private readonly IDefaultStrategyExpressionBuilder _builder;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="builder"></param>
        public GenericCompiledDecoratorStrategy(Type activationType, IInjectionScope injectionScope, IDefaultStrategyExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.DecoratorStrategy;

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var configuration = ActivationConfiguration.CloneToType(closedType);
            
            return configuration;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var activationType = request.ActivationType;

            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var configuration = GetActivationConfiguration(closedType);

            return _builder.GetActivationExpression(scope, request, configuration, lifestyle);
        }

        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        public bool ApplyAfterLifestyle { get; set; }
    }
}
