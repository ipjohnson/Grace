using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    /// <summary>
    /// Strategy for creating List(T)
    /// </summary>
    public class ListEnumerableStrategy : BaseGenericEnumerableStrategy
    {
        /// <summary>
        /// Default cosntructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public ListEnumerableStrategy( IInjectionScope injectionScope) : base(typeof(List<>), injectionScope)
        {
            AddExportAs(typeof(List<>));
            AddExportAs(typeof(IList<>));
            AddExportAs(typeof(ICollection<>));
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

            var closedType = typeof(List<>).MakeGenericType(elementType);

            var newRequest = request.NewRequest(elementType.MakeArrayType(), this, closedType, RequestType.Other, null, true, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var listResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);


            var constructor = closedType.GetTypeInfo().DeclaredConstructors.First(c =>
            {
                var parameters = c.GetParameters();

                if (parameters.Length == 1)
                {
                    return parameters[0].ParameterType.IsConstructedGenericType &&
                           parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                }

                return false;
            });

            var expression = Expression.New(constructor, listResult.Expression);

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.AddExpressionResult(listResult);

            return result;
        }

    }
}
