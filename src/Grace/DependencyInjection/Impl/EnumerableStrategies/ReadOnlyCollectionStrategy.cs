using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    /// <summary>
    /// Strategy for creating ReadOnly(T) 
    /// </summary>
    public class ReadOnlyCollectionStrategy : BaseGenericEnumerableStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public ReadOnlyCollectionStrategy(IInjectionScope injectionScope) : base(typeof(ReadOnlyCollection<>), injectionScope)
        {
            AddExportAs(typeof(ReadOnlyCollection<>));
            AddExportAs(typeof(IReadOnlyList<>));
            AddExportAs(typeof(IReadOnlyCollection<>));
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

            var closedType = typeof(ReadOnlyCollection<>).MakeGenericType(elementType);

            var newRequest = request.NewRequest(typeof(IList<>).MakeGenericType(elementType), this,closedType , RequestType.Other, null, true);

            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var listResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var constructor = closedType.GetTypeInfo().DeclaredConstructors.First(c =>
            {
                var parameters = c.GetParameters();

                if (parameters.Length == 1)
                {
                    return parameters[0].ParameterType.IsConstructedGenericType &&
                           parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IList<>);
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
