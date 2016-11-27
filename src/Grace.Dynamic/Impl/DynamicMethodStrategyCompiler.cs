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
    public class DynamicMethodStrategyCompiler : ActivationStrategyCompiler
    {
        private readonly ILinqToDynamicMethodConverter _linqToDynamicMethodConverter;

        public DynamicMethodStrategyCompiler(IInjectionScopeConfiguration configuration, IActivationExpressionBuilder builder, IAttributeDiscoveryService attributeDiscoveryService, ILifestyleExpressionBuilder exportExpressionBuilder, IInjectionContextCreator injectionContextCreator, IExpressionConstants constants, ILinqToDynamicMethodConverter linqToDynamicMethodConverter) : 
            base(configuration, builder, attributeDiscoveryService, exportExpressionBuilder, injectionContextCreator, constants)
        {
            _linqToDynamicMethodConverter = linqToDynamicMethodConverter;
        }

        protected override ActivationStrategyDelegate CompileExpressionResultToDelegate(IActivationExpressionResult expressionContext,
            ParameterExpression[] parameters, Expression[] extraExpressions, Expression finalExpression)
        {
            if (parameters.Length == 0 &&
                extraExpressions.Length == 0)
            {
                ActivationStrategyDelegate dynamicDelegate;

                if (_linqToDynamicMethodConverter.TryCreateDelegate(expressionContext, finalExpression, out dynamicDelegate))
                {
                    return dynamicDelegate;
                }
            }

            return base.CompileExpressionResultToDelegate(expressionContext, parameters, extraExpressions, finalExpression);
        }
    }
}
