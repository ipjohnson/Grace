using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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
            if(!(@delegate is Delegate)) throw new ArgumentException("parameter must be type of delegate",nameof(@delegate));

            _delegate = @delegate;
        }

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

        protected virtual IActivationExpressionResult CreateExpression(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var methodInfo = DelegateProperty.GetMethodInfo();

            var resultsExpressions = CreateExpressionsForTypes(scope, request, methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            Expression expression = Expression.Call(DelegateProperty.Target == null ? null : Expression.Constant(DelegateProperty.Target), methodInfo, resultsExpressions.Select(e => e.Expression));

            expression = ApplyNullCheckAndAddDisposal(scope, request, expression);
            
            var result = request.Services.Compiler.CreateNewResult(request, expression);

            foreach (var expressionResult in resultsExpressions)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;
        }

        protected Delegate DelegateProperty => _delegate as Delegate;
    }
}
