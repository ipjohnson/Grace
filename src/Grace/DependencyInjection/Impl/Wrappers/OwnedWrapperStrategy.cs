using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Wrapper strategy for Owned&lt;T&gt;
    /// </summary>
    public class OwnedWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public OwnedWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Owned<>), injectionScope)
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

                if (genericType == typeof(Owned<>))
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
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var constructor = request.ActivationType.GetTypeInfo().DeclaredConstructors.First();

            var wrappedType = request.ActivationType.GenericTypeArguments[0];
            var ownedParameter = Expression.Parameter(request.ActivationType);

            var assign = Expression.Assign(ownedParameter, Expression.New(constructor));

            var newRequest = request.NewRequest(wrappedType, this, request.ActivationType, RequestType.Other, null, true, true);

            newRequest.DisposalScopeExpression = ownedParameter;

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var setMethod = request.ActivationType.GetRuntimeMethods().First(m => m.Name == "SetValue");

            var expression = Expression.Call(ownedParameter, setMethod, expressionResult.Expression);

            var returnExpression = request.Services.Compiler.CreateNewResult(request, expression);

            returnExpression.AddExpressionResult(expressionResult);

            returnExpression.AddExtraParameter(ownedParameter);
            returnExpression.AddExtraExpression(assign);

            return returnExpression;
        }
    }
}
