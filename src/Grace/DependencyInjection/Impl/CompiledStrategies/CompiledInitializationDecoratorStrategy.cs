using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class CompiledInitializationDecoratorStrategy<T> : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private readonly Func<T, T> _func;

        public CompiledInitializationDecoratorStrategy( Func<T, T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _func = func;
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.DecoratorStrategy;

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var newRequest = request.NewRequest(typeof(T), this, ActivationType, RequestType.Other, null, true);

            var expression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var callExpression = Expression.Call(Expression.Constant(_func.Target), _func.GetMethodInfo(), expression.Expression);

            var resultExpression = request.Services.Compiler.CreateNewResult(request, callExpression);

            if (lifestyle != null)
            {
                resultExpression = lifestyle.ProvideLifestlyExpression(scope, request, resultExpression);
            }

            return resultExpression;
        }

        public bool ApplyAfterLifestyle { get; set; }
    }
}
