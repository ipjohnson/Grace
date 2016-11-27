using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.FactoryStrategies
{
    /// <summary>
    /// Strategy for Func that take no dependencies and returns TResult
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FactoryNoArgStrategy<TResult> : BaseInstanceExportStrategy
    {
        private readonly Func<TResult> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FactoryNoArgStrategy(Func<TResult> func , IInjectionScope injectionScope) : base(typeof(TResult), injectionScope)
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
            Expression expressionStatement = Expression.Call(_func.Target == null ? null : Expression.Constant(_func.Target), _func.GetMethodInfo());

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
