using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for Func that takes IExportLocatorScope and StaticInjectionContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncWithStaticContextInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly Func<IExportLocatorScope, StaticInjectionContext, T> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncWithStaticContextInstanceExportStrategy(Func<IExportLocatorScope,StaticInjectionContext,T> func, IInjectionScope injectionScope) : 
            base(typeof(T), injectionScope)
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
            var staticContext = request.GetStaticInjectionContext();

            Expression expressionStatement = 
                Expression.Call(Expression.Constant(_func.Target),
                                _func.GetMethodInfo(), 
                                request.Constants.ScopeParameter,
                                Expression.Constant(staticContext));

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
