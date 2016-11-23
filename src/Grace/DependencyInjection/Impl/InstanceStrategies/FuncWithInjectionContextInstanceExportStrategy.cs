using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy for a Func that takes locator scope, static injection context, and IInjectionContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncWithInjectionContextInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncWithInjectionContextInstanceExportStrategy(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T> func, IInjectionScope injectionScope) : 
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
            request.RequireInjectionContext();

            var staticContext = request.GetStaticInjectionContext();

            Expression expressionStatement =
                Expression.Call(Expression.Constant(_func.Target),
                                _func.GetMethodInfo(),
                                request.Constants.ScopeParameter,
                                Expression.Constant(staticContext),
                                request.Constants.InjectionContextParameter);

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
