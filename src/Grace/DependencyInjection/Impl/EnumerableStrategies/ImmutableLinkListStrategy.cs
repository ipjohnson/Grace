using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    /// <summary>
    /// Strategy for creating ImmutableLinkedList
    /// </summary>
    public class ImmutableLinkListStrategy : BaseGenericEnumerableStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public ImmutableLinkListStrategy(IInjectionScope injectionScope) : base(typeof(ImmutableLinkedList<>), injectionScope)
        {
        }
        
        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var elementType = request.ActivationType.GenericTypeArguments[0];

            var newRequest = request.NewRequest(elementType.MakeArrayType(), this, request.ActivationType, RequestType.Other, null, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var listResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var createMethod =
                typeof(ImmutableLinkListStrategy).GetTypeInfo().GetDeclaredMethod(nameof(CreateImmutableLinkedList));

            var closedMethod = createMethod.MakeGenericMethod(elementType);

            var expression = Expression.Call(closedMethod, listResult.Expression);

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expression);

            expressionResult.AddExpressionResult(listResult);

            return expressionResult;
        }

        /// <summary>
        /// Creates an immutable linked list from elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static ImmutableLinkedList<T> CreateImmutableLinkedList<T>(T[] elements)
        {
            var list = ImmutableLinkedList<T>.Empty;

            // reversing the order so that itterating over immutable gives the same order
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                list = list.Add(elements[i]);
            }

            return list;
        }
    }
}
