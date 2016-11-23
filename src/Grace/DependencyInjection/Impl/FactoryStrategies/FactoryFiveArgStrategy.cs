using System;
using System.Linq.Expressions;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take 5 dependencies and returns TResult
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryFiveArgStrategy<T1, T2, T3, T4, T5, TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<T1, T2, T3, T4, T5, TResult> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryFiveArgStrategy(Func<T1,T2,T3,T4,T5,TResult> func, IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
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

            var arg4Request = request.NewRequest(typeof(T4), this, typeof(TResult), RequestType.Other, null, true);

            var arg4Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg4Request);

            var arg5Request = request.NewRequest(typeof(T5), this, typeof(TResult), RequestType.Other, null, true);

            var arg5Result = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg5Request);

            Expression expression = Expression.Call(Expression.Constant(_func.Target),
                                             _func.GetMethodInfo(),
                                             arg1Result.Expression,
                                             arg2Result.Expression,
                                             arg3Result.Expression,
                                             arg4Result.Expression,
                                             arg5Result.Expression);

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(arg1Result);
            result.AddExpressionResult(arg2Result);
            result.AddExpressionResult(arg3Result);
            result.AddExpressionResult(arg4Result);
            result.AddExpressionResult(arg5Result);

            if (Lifestyle != null)
            {
                result = Lifestyle.ProvideLifestlyExpression(scope, request, result);
            }

            return result;
        }
    }
}
