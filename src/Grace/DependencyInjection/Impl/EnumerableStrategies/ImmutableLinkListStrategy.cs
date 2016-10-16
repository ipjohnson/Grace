using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    public class ImmutableLinkListStrategy : BaseGenericEnumerableStrategy
    {
        public ImmutableLinkListStrategy(IInjectionScope injectionScope) : base(typeof(ImmutableLinkedList<>), injectionScope)
        {
        }

        public override IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            throw new NotSupportedException("Decorators on collection is not supported at this time");
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var elementType = request.ActivationType.GenericTypeArguments[0];

            var newRequest = request.NewRequest(elementType.MakeArrayType(), this, request.ActivationType, RequestType.Other, null, true);

            var listResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var createMethod =
                typeof(ImmutableLinkListStrategy).GetTypeInfo().GetDeclaredMethod("CreateImmutableLinkedList");

            var closedMethod = createMethod.MakeGenericMethod(elementType);

            var expression = Expression.Call(closedMethod, listResult.Expression);

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expression);

            expressionResult.AddExpressionResult(listResult);

            return expressionResult;
        }

        public static ImmutableLinkedList<T> CreateImmutableLinkedList<T>(T[] elements)
        {
            var list = ImmutableLinkedList<T>.Empty;

            // reversing the order so that itterating over immutable gives the same order
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                list = list.Add(elements[i]);
            }

            return list;
        }
    }
}
