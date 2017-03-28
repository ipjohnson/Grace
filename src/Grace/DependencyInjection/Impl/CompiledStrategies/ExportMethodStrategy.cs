using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Export strategy for exporting a method off an existing type
    /// </summary>
    public class ExportMethodStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly ICompiledExportStrategy _dependentStrategy;
        private readonly MethodInfo _methodInfo;
        private ActivationStrategyDelegate _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="methodInfo"></param>
        /// <param name="dependentStrategy"></param>
        public ExportMethodStrategy(Type activationType, IInjectionScope injectionScope, ICompiledExportStrategy dependentStrategy, MethodInfo methodInfo) : base(activationType, injectionScope)
        {
            _methodInfo = methodInfo;
            _dependentStrategy = dependentStrategy;
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            return GetExpressionFromDependentStrategy(scope, request);
        }


        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            if (_delegate != null)
            {
                return _delegate;
            }

            var request = GetActivationExpression(scope, compiler.CreateNewRequest(activationType, 1, scope));

            var compiledDelegate = compiler.CompileDelegate(scope, request);

            Interlocked.CompareExchange(ref _delegate, compiledDelegate, null);

            return _delegate;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this);

            return result ?? GetExpressionFromDependentStrategy(scope, request);
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            throw new NotSupportedException("Secondary strategies not supported on method export");
        }

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }

        private IActivationExpressionResult GetExpressionFromDependentStrategy(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var newRequest = request.NewRequest(_dependentStrategy.ActivationType, this, request.InjectedType, request.RequestType, request.Info, true, true);

            var instanceResult = _dependentStrategy.GetActivationExpression(scope, newRequest);

            var results = new List<IActivationExpressionResult> { instanceResult };
            var parameterExpressions = new List<Expression>();

            foreach (var parameterInfo in _methodInfo.GetParameters())
            {
                newRequest = request.NewRequest(parameterInfo.ParameterType, this, _dependentStrategy.ActivationType,
                    RequestType.MethodParameter, parameterInfo, false, true);

                var parameterResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                results.Add(parameterResult);
                parameterExpressions.Add(parameterResult.Expression);
            }

            var expression = Expression.Call(instanceResult.Expression, _methodInfo, parameterExpressions);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            foreach (var expressionResult in results)
            {
                result.AddExpressionResult(expressionResult);
            }

            return result;
        }
    }
}
