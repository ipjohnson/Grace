using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    public class FuncInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly Func<T> _func;

        public FuncInstanceExportStrategy(Func<T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _func = func;
        }
        
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            Expression expressionStatement = Expression.Call(Expression.Constant(_func.Target), _func.GetMethodInfo());

            expressionStatement = ApplyNullCheckAndAddDisposal(scope, request, expressionStatement);

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expressionStatement);

            if (lifestyle != null)
            {
                expressionResult = lifestyle.ProvideLifestlyExpression(scope, request, expressionResult);
            }

            return expressionResult;
        }
    }
}
