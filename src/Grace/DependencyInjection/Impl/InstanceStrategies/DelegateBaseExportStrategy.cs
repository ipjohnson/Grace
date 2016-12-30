using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Base class for all delegate based strategies (i.e. FactoryOneArgStrategy etc)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateBaseExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly object _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="delegate">delegate instance</param>
        public DelegateBaseExportStrategy(Type activationType, IInjectionScope injectionScope, object @delegate) : base(activationType, injectionScope)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            if (!(@delegate is Delegate)) throw new ArgumentException("parameter must be type of delegate", nameof(@delegate));

            _delegate = @delegate;
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
            if (lifestyle == null)
            {
                return CreateExpression(scope, request);
            }

            return lifestyle.ProvideLifestlyExpression(scope, request,
                expressionRequest => CreateExpression(scope, expressionRequest));
        }

        /// <summary>
        /// Create expression that calls a delegate
        /// </summary>
        /// <param name="scope">scope for the request</param>
        /// <param name="request">activation request</param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateExpression(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var methodInfo = DelegateInstance.GetMethodInfo();

            var resultsExpressions = CreateExpressionsForTypes(scope, request, methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            Expression expression = Expression.Call(DelegateInstance.Target == null ? null : Expression.Constant(DelegateInstance.Target), methodInfo, resultsExpressions.Select(e => e.Expression));

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            foreach (var expressionResult in resultsExpressions)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;
        }

        /// <summary>
        /// Delegate to call
        /// </summary>
        protected Delegate DelegateInstance => _delegate as Delegate;
    }
}
