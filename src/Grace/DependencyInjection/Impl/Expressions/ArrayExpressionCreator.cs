using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Interface for creating array expressions for a given request
    /// </summary>
    public interface IArrayExpressionCreator
    {
        /// <summary>
        /// Get linq expression to create
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <returns></returns>
        IActivationExpressionResult GetArrayExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }

    /// <summary>
    /// Creates linq expression for array initialization
    /// </summary>
    public class ArrayExpressionCreator : IArrayExpressionCreator
    {
        private readonly IWrapperExpressionCreator _wrapperExpressionCreator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wrapperExpressionCreator"></param>
        public ArrayExpressionCreator(IWrapperExpressionCreator wrapperExpressionCreator)
        {
            _wrapperExpressionCreator = wrapperExpressionCreator;
        }

        /// <summary>
        /// Get linq expression to create
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get list of expressions to populate array
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        protected virtual List<IActivationExpressionResult> GetArrayExpressionList(IInjectionScope scope, IActivationExpressionRequest request, Type arrayElementType)
        {
            var expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, false);

            if (expressions.Count != 0)
            {
                return expressions;
            }

            lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
            {
                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, true);

                if (expressions.Count != 0)
                {
                    return expressions;
                }

                request.Services.Compiler.ProcessMissingStrategyProviders(scope, request.Services.Compiler.CreateNewRequest(arrayElementType, request.ObjectGraphDepth + 1, scope));

                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, true);
            }

            return expressions;
        }

        /// <summary>
        /// Get activation expression for export strategies
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="locked"></param>
        /// <returns></returns>
        protected virtual List<IActivationExpressionResult> GetActivationExpressionResultsFromStrategies(IInjectionScope scope, IActivationExpressionRequest request,
            Type arrayElementType, bool locked)
        {
            var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(arrayElementType);
            var expressions = new List<IActivationExpressionResult>();

            if (collection != null)
            {
                foreach (var strategy in collection.GetStrategies())
                {
                    var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType, request.RequestType,
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
                        var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType, request.RequestType,
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

        /// <summary>
        /// Process wrappers looking for matching type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="request"></param>
        /// <param name="expressions"></param>
        /// <param name="locked"></param>
        protected virtual void ProcessWrappers(IInjectionScope scope, Type arrayElementType, IActivationExpressionRequest request, List<IActivationExpressionResult> expressions, bool locked)
        {
            Type wrappedType;
            var wrappers = _wrapperExpressionCreator.GetWrappers(scope, arrayElementType,request, out wrappedType);

            if (wrappers != ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                wrappers = wrappers.Reverse();

                GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                if (expressions.Count == 0)
                {
                    if (locked)
                    {
                        lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
                        {
                            GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                            if (expressions.Count == 0)
                            {
                                var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType, RequestType.Other, null, true);

                                request.Services.Compiler.ProcessMissingStrategyProviders(scope, newRequest);

                                GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);
                            }
                        }
                    }
                    else
                    {
                        var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType,
                                    RequestType.Other, null, true);

                        request.Services.Compiler.ProcessMissingStrategyProviders(scope, newRequest);

                        GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                    }
                }
            }
        }

        /// <summary>
        /// Get expression from collections
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="request"></param>
        /// <param name="expressions"></param>
        /// <param name="wrappedType"></param>
        /// <param name="wrappers"></param>
        public static void GetExpressionsFromCollections(IInjectionScope scope, Type arrayElementType,
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

        /// <summary>
        /// Get expression from an activation strategy collection
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="request"></param>
        /// <param name="expressions"></param>
        /// <param name="collection"></param>
        /// <param name="wrappedType"></param>
        /// <param name="wrappers"></param>
        public static void GetExpressionFromCollection(IInjectionScope scope, Type arrayElementType,
            IActivationExpressionRequest request, List<IActivationExpressionResult> expressions, IActivationStrategyCollection<ICompiledExportStrategy> collection, Type wrappedType,
            ImmutableLinkedList<IActivationPathNode> wrappers)
        {
            foreach (var strategy in collection.GetStrategies())
            {
                if (strategy.HasConditions)
                {
                    var staticContext = request.GetStaticInjectionContext();
                    var pass = true;

                    foreach (var condition in strategy.Conditions)
                    {
                        if (!condition.MeetsCondition(strategy, staticContext))
                        {
                            pass = false;
                            break;
                        }                           
                    }

                    if (!pass)
                    {
                        continue;
                    }
                }

                var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType,
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
