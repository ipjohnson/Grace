using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Decorator strategy for Func
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompiledInitializationDecoratorStrategy<T> : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private readonly Func<T, T> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public CompiledInitializationDecoratorStrategy(Func<T, T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _func = func;
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
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
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
        /// Internal method for creating acivation expression
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult InternalGetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var newRequest = request.NewRequest(typeof(T), this, ActivationType, RequestType.Other, null, true, true);

            var expression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var callExpression = Expression.Call(Expression.Constant(_func.Target), _func.GetMethodInfo(), expression.Expression);

            var resultExpression = request.Services.Compiler.CreateNewResult(request, callExpression);

            return resultExpression;
        }

        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        public bool ApplyAfterLifestyle { get; set; }
    }
}
