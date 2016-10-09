using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    public class FactoryNoArgStrategy<TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<TResult> _func;

        public FactoryNoArgStrategy(Func<TResult> func , IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
        {
            _func = func;
        }

        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
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
