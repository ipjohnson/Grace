using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Strategy to export a property of field from another strategy
    /// </summary>
    public class ExportedPropertyOrFieldStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly ICompiledExportStrategy _dependentStrategy;
        private readonly string _propertyOrFieldName;
        private ActivationStrategyDelegate _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="dependentStrategy"></param>
        /// <param name="propertyOrFieldName"></param>
        public ExportedPropertyOrFieldStrategy(Type activationType, IInjectionScope injectionScope,
            ICompiledExportStrategy dependentStrategy, string propertyOrFieldName)
            : base(activationType, injectionScope)
        {
            _dependentStrategy = dependentStrategy;
            _propertyOrFieldName = propertyOrFieldName;
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
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request,
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
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope,
            IActivationStrategyCompiler compiler,
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
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope,
                request, this);

            return result ?? GetExpressionFromDependentStrategy(scope, request);
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            throw new NotSupportedException("Secondary strategies not supported on property export");
        }

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }

        private IActivationExpressionResult GetExpressionFromDependentStrategy(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var newRequest = request.NewRequest(_dependentStrategy.ActivationType, this, request.InjectedType,
                request.RequestType, request.Info, true, true);

            var instanceResult = _dependentStrategy.GetActivationExpression(scope, newRequest);

            Expression propertyExpression;

            try
            {
                propertyExpression = Expression.PropertyOrField(instanceResult.Expression, _propertyOrFieldName);
            }
            catch (Exception exp)
            {
                throw new LocateException(request.GetStaticInjectionContext(), exp, $"Could not create property/field expression for {_propertyOrFieldName} on type {instanceResult.Expression.Type.FullName}");
            }

            var expressionResult = request.Services.Compiler.CreateNewResult(request, propertyExpression);

            expressionResult.AddExpressionResult(instanceResult);

            return expressionResult;
        }

    }
}
