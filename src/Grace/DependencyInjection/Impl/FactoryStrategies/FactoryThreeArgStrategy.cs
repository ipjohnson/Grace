using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 3 dependencies and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryThreeArgStrategy<T1, T2, T3, TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<T1, T2, T3, TResult> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryThreeArgStrategy(Func<T1,T2,T3,TResult> func , IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
        {
            _func = func;
        }

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var arg1Request = request.NewRequest(typeof(T1), this, typeof(TResult), RequestType.Other, null, true);

            var arg1Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg1Request);

            var arg2Request = request.NewRequest(typeof(T2), this, typeof(TResult), RequestType.Other, null, true);

            var arg2Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg2Request);

            var arg3Request = request.NewRequest(typeof(T3), this, typeof(TResult), RequestType.Other, null, true);

            var arg3Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg3Request);

            Expression expression = Expression.Call(_func.Target == null ? null : Expression.Constant(_func.Target), _func.GetMethodInfo(), arg1Result.Expression, arg2Result.Expression, arg3Result.Expression);

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(arg1Result);
            result.AddExpressionResult(arg2Result);
            result.AddExpressionResult(arg3Result);

            if (Lifestyle != null)
            {
                result = Lifestyle.ProvideLifestlyExpression(scope, request, result);
            }

            return result;
        }
    }
}
