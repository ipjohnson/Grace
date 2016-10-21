using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    public class FactoryOneArgStrategy<T1, TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<T1, TResult> _func;

        public FactoryOneArgStrategy(Func<T1, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
        {
            _func = func;
        }

        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var newRequest = request.NewRequest(typeof(T1), this, typeof(TResult), RequestType.Other, null, true);

            var argResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            Expression expression = Expression.Call(Expression.Constant(_func.Target), _func.GetMethodInfo(), argResult.Expression);

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(argResult);

            if (Lifestyle != null)
            {
                result = Lifestyle.ProvideLifestlyExpression(scope, request, result);
            }

            return result;
        }
    }
}
