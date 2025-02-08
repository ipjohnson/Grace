﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    /// <summary>
    /// Strategy for creating System.Collections.Immutable 
    /// </summary>
    public class ImmutableCollectionStrategy : BaseGenericEnumerableStrategy
    {
        private readonly MethodInfo _createMethod;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        public ImmutableCollectionStrategy(Type activationType, IInjectionScope injectionScope) 
            : base(activationType.GetGenericTypeDefinition(), injectionScope)
        {
            var staticType = activationType.Assembly
                .GetType(activationType.FullName.Replace("`1", ""));

            _createMethod = staticType
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Create" && m.GetParameters() is [{ ParameterType.IsArray: true }]);
        }


        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var elementType = request.ActivationType.GenericTypeArguments[0];

            var newRequest = request.NewRequest(elementType.MakeArrayType(), this, request.ActivationType, RequestType.Other, null, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var arrayResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);
            
            var closedMethod = _createMethod.MakeGenericMethod(elementType);

            var expression = Expression.Call(closedMethod, arrayResult.Expression);

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expression);

            expressionResult.AddExpressionResult(arrayResult);

            return expressionResult;
        }
    }
}
