﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Wrapper for Meta&lt;T&gt;
    /// </summary>
    public class MetaWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public MetaWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Meta<>), injectionScope)
        {
        }

        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="wrappedType">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        public override Type GetWrappedType(Type wrappedType)
        {
            if (wrappedType.IsConstructedGenericType)
            {
                var genericType = wrappedType.GetGenericTypeDefinition();

                if (genericType == typeof(Meta<>))
                {
                    return wrappedType.GetTypeInfo().GenericTypeArguments[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var requestType = request.ActivationType.GetTypeInfo().GenericTypeArguments[0];

            var constructor = request.ActivationType.GetTypeInfo().DeclaredConstructors.First();

            var newRequest = request.NewRequest(requestType, this, request.ActivationType, RequestType.Other, null, true, true);

            var strategy = request.GetWrappedExportStrategy();

            if (strategy == null)
            {
                throw new Exception("Could not find export stragegy to get metadata from");
            }

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var newExpression =
                Expression.New(constructor, expressionResult.Expression, Expression.Constant(strategy.Metadata));

            var newResult = request.Services.Compiler.CreateNewResult(request, newExpression);

            newResult.AddExpressionResult(expressionResult);

            return newResult;
        }

        /// <summary>
        /// Compiles delegate
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="request"></param>
        protected override ActivationStrategyDelegate CompileDelegate(
            IActivationStrategyCompiler compiler,
            IActivationExpressionRequest request)
        {

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(request.RequestingScope, request);

            return compiler.CompileDelegate(request.RequestingScope, expressionResult);
        }
    }
}
