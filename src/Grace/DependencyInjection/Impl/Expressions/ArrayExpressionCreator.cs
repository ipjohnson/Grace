using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IArrayExpressionCreator
    {
        IActivationExpressionResult GetArrayExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }

    public class ArrayExpressionCreator : IArrayExpressionCreator
    {
        private readonly IWrapperExpressionCreator _wrapperExpressionCreator;

        public ArrayExpressionCreator(IWrapperExpressionCreator wrapperExpressionCreator)
        {
            _wrapperExpressionCreator = wrapperExpressionCreator;
        }

        public IActivationExpressionResult GetArrayExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var arrayElementType = request.ActivationType.GetElementType();

            var arrayExpressionList = GetArrayExpressionList(scope, request, arrayElementType);

            var arrayInit = Expression.NewArrayInit(arrayElementType, arrayExpressionList.Select(e => e.Expression));

            var returnResult = request.Services.Compiler.CreateNewResult(request, arrayInit);

            foreach (var result in arrayExpressionList)
            {
                returnResult.AddExpressionResult(result);
            }

            return returnResult;
        }

        private List<IActivationExpressionResult> GetArrayExpressionList(IInjectionScope scope, IActivationExpressionRequest request, Type arrayElementType)
        {
            var expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, false);

            if (expressions.Count != 0)
            {
                return expressions;
            }

            lock (scope.GetLockObject(RootInjectionScope.ActivationStrategyAddLockName))
            {
                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, true);

                if (expressions.Count != 0)
                {
                    return expressions;
                }

                request.Services.Compiler.ProcessMissingStrategyProviders(scope, request.Services.Compiler.CreateNewRequest(arrayElementType, request.ObjectGraphDepth + 1));

                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, true);
            }

            return expressions;
        }

        private List<IActivationExpressionResult> GetActivationExpressionResultsFromStrategies(IInjectionScope scope, IActivationExpressionRequest request,
            Type arrayElementType, bool locked)
        {
            var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(arrayElementType);
            var expressions = new List<IActivationExpressionResult>();

            if (collection != null)
            {
                foreach (var strategy in collection.GetStrategies())
                {
                    var newRequest = request.NewRequest(arrayElementType, request.InjectedType, request.RequestType,
                        request.Info, true);

                    expressions.Add(strategy.GetActivationExpression(scope, newRequest));
                }
            }

            // check for generic
            if (arrayElementType.IsConstructedGenericType)
            {
                var genericType = arrayElementType.GetGenericTypeDefinition();

                var strategies = scope.StrategyCollectionContainer.GetActivationStrategyCollection(genericType);

                if (strategies != null)
                {
                    foreach (var strategy in strategies.GetStrategies())
                    {
                        var newRequest = request.NewRequest(arrayElementType, request.InjectedType, request.RequestType,
                            request.Info, true);

                        expressions.Add(strategy.GetActivationExpression(scope, newRequest));
                    }
                }
            }

            if (expressions.Count == 0)
            {
                ProcessWrappers(scope, arrayElementType, request, expressions, locked);
            }

            return expressions;
        }

        private void ProcessWrappers(IInjectionScope scope, Type arrayElementType, IActivationExpressionRequest request, List<IActivationExpressionResult> expressions, bool locked)
        {
            Type wrappedType;
            var wrappers = _wrapperExpressionCreator.GetWrappers(scope, arrayElementType, out wrappedType);

            if (wrappers != ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                wrappers = wrappers.Reverse();

                GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                if (expressions.Count == 0)
                {
                    if (locked)
                    {
                        lock (scope.GetLockObject(RootInjectionScope.ActivationStrategyAddLockName))
                        {
                            GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                            if (expressions.Count == 0)
                            {
                                var newRequest = request.NewRequest(arrayElementType, request.InjectedType,
                                    RequestType.Other, null, true);

                                request.Services.Compiler.ProcessMissingStrategyProviders(scope, newRequest);

                                GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);
                            }
                        }
                    }
                    else
                    {
                        var newRequest = request.NewRequest(arrayElementType, request.InjectedType,
                                    RequestType.Other, null, true);

                        request.Services.Compiler.ProcessMissingStrategyProviders(scope, newRequest);

                        GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                    }
                }
            }
        }

        private static void GetExpressionsFromCollections(IInjectionScope scope, Type arrayElementType,
            IActivationExpressionRequest request, List<IActivationExpressionResult> expressions, Type wrappedType, ImmutableLinkedList<IActivationPathNode> wrappers)
        {
            var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(wrappedType);

            if (collection != null)
            {
                GetExpressionFromCollection(scope, arrayElementType, request, expressions, collection, wrappedType, wrappers);
            }

            var isGenericType = wrappedType.IsConstructedGenericType;

            if (isGenericType)
            {
                var genericType = wrappedType.GetGenericTypeDefinition();

                collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(genericType);

                if (collection != null)
                {
                    GetExpressionFromCollection(scope, arrayElementType, request, expressions, collection, wrappedType, wrappers);
                }
            }
        }

        private static void GetExpressionFromCollection(IInjectionScope scope, Type arrayElementType,
            IActivationExpressionRequest request, List<IActivationExpressionResult> expressions, IActivationStrategyCollection<ICompiledExportStrategy> collection, Type wrappedType,
            ImmutableLinkedList<IActivationPathNode> wrappers)
        {
            foreach (var strategy in collection.GetStrategies())
            {
                if (strategy.HasConditions)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var newRequest = request.NewRequest(arrayElementType, request.InjectedType,
                        RequestType.Other, null, true);

                    var newPath =
                        ImmutableLinkedList<IActivationPathNode>.Empty.Add(
                            new WrapperActivationPathNode(strategy,
                                wrappedType, null)).AddRange(wrappers);

                    newRequest.SetWrapperPath(newPath);

                    var wrapper = newRequest.PopWrapperPathNode();

                    expressions.Add(wrapper.GetActivationExpression(scope, newRequest));
                }
            }
        }
    }
}
