using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grace.Data.Immutable;
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
        /// <param name="linqToDynamicMethodConverter"></param>
        public DynamicMethodStrategyCompiler(IInjectionScopeConfiguration configuration, IActivationExpressionBuilder builder, IAttributeDiscoveryService attributeDiscoveryService, ILifestyleExpressionBuilder exportExpressionBuilder, IInjectionContextCreator injectionContextCreator, IExpressionConstants constants, ILinqToDynamicMethodConverter linqToDynamicMethodConverter) : 
            base(configuration, builder, attributeDiscoveryService, exportExpressionBuilder, injectionContextCreator, constants)
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
            if (parameters.Length == 0 &&
                extraExpressions.Length == 0)
            {
                ActivationStrategyDelegate dynamicDelegate;

                // try to create delegate, if not fall back to normal Linq Expression compile
                if (_linqToDynamicMethodConverter.TryCreateDelegate(expressionContext, finalExpression, out dynamicDelegate))
                {
                    return dynamicDelegate;
                }
            }

            return base.CompileExpressionResultToDelegate(expressionContext, parameters, extraExpressions, finalExpression);
        }
    }
}
