using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy that represents Func with no arguemnts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly Func<T> _func;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope"></param>
        public FuncInstanceExportStrategy(Func<T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _func = func;
        }

        public override ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            if (StrategyDelegate != null)
            {
                return StrategyDelegate;
            }

            return Lifestyle == null ? CreateDelegate() : base.GetActivationStrategyDelegate(scope, compiler, activationType);
        }

        private ActivationStrategyDelegate CreateDelegate()
        {
            var staticContext = new StaticInjectionContext(typeof(T));

            if (ExternallyOwned || !typeof(T).GetTypeInfo().IsAssignableFrom(typeof(IDisposable).GetTypeInfo()))
            {
                StrategyDelegate = (scope, disposalScope, context) => CheckForNull(staticContext, _func());
            }
            else
            {
                StrategyDelegate = (scope, disposalScope, context) => CheckForNullAndAddToDisposalScope(disposalScope, staticContext, _func());
            }

            return StrategyDelegate;
        }

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
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
