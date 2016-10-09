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
    public class FactoryFourArgStrategy<T1, T2, T3, T4, TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<T1, T2, T3, T4, TResult> _func;

        public FactoryFourArgStrategy(Func<T1, T2, T3, T4, TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
        {
            _func = func;
        }

        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var arg1Request = request.NewRequest(typeof(T1), typeof(TResult), RequestType.Other, null, true);

            var arg1Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg1Request);

            var arg2Request = request.NewRequest(typeof(T2), typeof(TResult), RequestType.Other, null, true);

            var arg2Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg2Request);

            var arg3Request = request.NewRequest(typeof(T3), typeof(TResult), RequestType.Other, null, true);

            var arg3Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg3Request);

            var arg4Request = request.NewRequest(typeof(T4), typeof(TResult), RequestType.Other, null, true);

            var arg4Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg4Request);

            Expression expression = Expression.Call(Expression.Constant(_func.Target), 
                                             _func.GetMethodInfo(), 
                                             arg1Result.Expression, 
                                             arg2Result.Expression, 
                                             arg3Result.Expression,
                                             arg4Result.Expression);

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(arg1Result);
            result.AddExpressionResult(arg2Result);
            result.AddExpressionResult(arg3Result);
            result.AddExpressionResult(arg4Result);

            if (Lifestyle != null)
            {
                result = Lifestyle.ProvideLifestlyExpression(scope, request, result);
            }

            return result;
        }
    }
}
