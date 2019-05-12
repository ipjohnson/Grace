using System.Linq.Expressions;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.Dynamic.Impl
{
    /// <summary>
    /// strategy compiler class that generates ActivationStrategyDelegate using IL 
    /// </summary>
    public class DynamicMethodStrategyCompiler : ActivationStrategyCompiler
    {
        private readonly ILinqToDynamicMethodConverter _linqToDynamicMethodConverter;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="builder"></param>
        /// <param name="attributeDiscoveryService"></param>
        /// <param name="exportExpressionBuilder"></param>
        /// <param name="injectionContextCreator"></param>
        /// <param name="constants"></param>
        /// <param name="injectionCreator"></param>
        /// <param name="linqToDynamicMethodConverter"></param>
        public DynamicMethodStrategyCompiler(IInjectionScopeConfiguration configuration, IActivationExpressionBuilder builder, IAttributeDiscoveryService attributeDiscoveryService, IDefaultStrategyExpressionBuilder exportExpressionBuilder, IInjectionContextCreator injectionContextCreator, IExpressionConstants constants, IInjectionStrategyDelegateCreator injectionCreator, ILinqToDynamicMethodConverter linqToDynamicMethodConverter) :
            base(configuration, builder, attributeDiscoveryService, exportExpressionBuilder, injectionContextCreator, constants, injectionCreator)
        {
            _linqToDynamicMethodConverter = linqToDynamicMethodConverter;
        }

        /// <summary>
        /// Compiles an expression result to a delegate
        /// </summary>
        /// <param name="expressionContext"></param>
        /// <param name="parameters"></param>
        /// <param name="extraExpressions"></param>
        /// <param name="finalExpression"></param>
        /// <returns></returns>
        protected override ActivationStrategyDelegate CompileExpressionResultToDelegate(IActivationExpressionResult expressionContext,
            ParameterExpression[] parameters, Expression[] extraExpressions, Expression finalExpression)
        {
            ActivationStrategyDelegate dynamicDelegate =
                (ActivationStrategyDelegate)_linqToDynamicMethodConverter.TryCreateDelegate(expressionContext, parameters, extraExpressions, finalExpression, typeof(ActivationStrategyDelegate));

            if (dynamicDelegate != null)
            {
                return dynamicDelegate;
            }

            expressionContext.Request.RequestingScope.ScopeConfiguration.Trace?.Invoke($"Could not generate delegate for {expressionContext.Request.ActivationType.FullName} using DynamicMethod falling back to linq expressions");

            return base.CompileExpressionResultToDelegate(expressionContext, parameters, extraExpressions, finalExpression);
        }

        protected override T CompileExpressionResultToOpitimzed<T>(IActivationExpressionResult expressionContext, ParameterExpression[] parameters,
            Expression[] extraExpressions, Expression finalExpression)
        {
            T delegateValue = 
                (T)(object)_linqToDynamicMethodConverter.TryCreateDelegate(expressionContext, parameters, extraExpressions, finalExpression, typeof(T));

            if (delegateValue != null)
            {
                return delegateValue;
            }

            expressionContext.Request.RequestingScope.ScopeConfiguration.Trace?.Invoke($"Could not generate delegate for {expressionContext.Request.ActivationType.FullName} using DynamicMethod falling back to linq expressions");

            return base.CompileExpressionResultToOpitimzed<T>(expressionContext, parameters, extraExpressions, finalExpression);
        }
    }
}
