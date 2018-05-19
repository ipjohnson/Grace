using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class CompiledFactoryDecoratorStrategy<T> : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private Delegate _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="delegate"></param>
        /// <param name="injectionScope">owning injection scope</param>
        public CompiledFactoryDecoratorStrategy(Delegate @delegate, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _delegate = @delegate;
        }

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
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            if (lifestyle == null)
            {
                return InternalGetDecoratorActivationExpression(scope, request);
            }

            if (ApplyAfterLifestyle)
            {
                return lifestyle.ProvideLifestyleExpression(
                    scope, request, lifestyleRequest => InternalGetDecoratorActivationExpression(scope, lifestyleRequest));
            }

            return lifestyle.ProvideLifestyleExpression(
                scope, request, lifestyleRequest => InternalGetDecoratorActivationExpression(scope, request));
        }

        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        public bool ApplyAfterLifestyle { get; set; }

        /// <summary>
        /// Get decorator expression
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult InternalGetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return ExpressionUtilities.CreateExpressionForDelegate(_delegate, false, InjectionScope, request, this);
        }
    }
}
