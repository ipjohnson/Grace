using Optional;
using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using Grace.DependencyInjection.Impl.Wrappers;
using System.Linq.Expressions;
using System.Linq;

namespace Grace.Tests.ThirdParty.Optional
{
    /// <summary>
    /// Optional strategy provider
    /// </summary>
    public class OptionalStrategyProvider : IMissingExportStrategyProvider
    {
        /// <summary>
        /// Can a given request be located using this provider
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return request.ActivationType.IsConstructedGenericType &&
                   request.ActivationType.GetGenericTypeDefinition() == typeof(Option<>);
        }

        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.ActivationType.IsConstructedGenericType &&
                request.ActivationType.GetGenericTypeDefinition() == typeof(Option<>))
            {
                yield return new OptionalWrapperStrategy(scope);
            }
        }
    }

    public class OptionalWrapperStrategy(IInjectionScope scope) 
        : BaseWrapperStrategy(typeof(Option<>), scope)
    {
        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="type">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        public override Type GetWrappedType(Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Option<>) 
                ? type.GenericTypeArguments[0]
                : null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope">IInjectionScope</param>
        /// <param name="request">IActivationExpressionRequest</param>
        /// <returns>IActivationExpressionResult</returns>
        public override IActivationExpressionResult GetActivationExpression(
            IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            var wrappedType = GetWrappedType(request.ActivationType);                    

            var newRequest = request.NewRequest(wrappedType, this, request.ActivationType, RequestType.Other, null, true, true);
            newRequest.SetIsRequired(false);

            var wrappedResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            Expression optionExpression;

            if (wrappedResult.UsingFallbackExpression)
            {
                var method = typeof(Option)
                    .GetMethods()
                    .First(m => m.Name == "None" && m.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(wrappedType);
                optionExpression = Expression.Call(method);
            }
            else
            {
                var method = typeof(Option)
                    .GetMethods()
                    .First(m => m.Name == "Some" && m.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(wrappedType);
                optionExpression = Expression.Call(method, wrappedResult.Expression);
            }

            var result = request.Services.Compiler.CreateNewResult(request, optionExpression);
            result.AddExpressionResult(wrappedResult);

            return result;
        }
    }
}
