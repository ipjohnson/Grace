using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    /// <summary>
    /// Strategy for creating ImmutableArray
    /// </summary>
    public class ImmutableArrayStrategy : BaseGenericEnumerableStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public ImmutableArrayStrategy(IInjectionScope injectionScope) : base(typeof(ImmutableArray<>), injectionScope)
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

            var fromMethod =
                typeof(ImmutableArray).GetRuntimeMethods().First(m => m.Name == nameof(ImmutableArray.From) && m.GetParameters().Length == 2);

            var closedMethod = fromMethod.MakeGenericMethod(elementType);

            var expression = Expression.Call(closedMethod, listResult.Expression, Expression.Constant(-1));

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(listResult);

            return result;
        }

    }
}
