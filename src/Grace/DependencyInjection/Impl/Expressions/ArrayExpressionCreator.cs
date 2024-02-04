using System;
using System.Collections;
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
        /// <param name="rootKey">key for keyed Root requests</param>
        IActivationExpressionResult GetArrayExpression(IInjectionScope scope, IActivationExpressionRequest request, object rootKey);
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
        /// <param name="rootKey">key for keyed Root requests</param>
        public IActivationExpressionResult GetArrayExpression(IInjectionScope scope, IActivationExpressionRequest request, object rootKey)
        {
            var arrayElementType = request.ActivationType.GetElementType();

            var arrayExpressionList = GetArrayExpressionList(scope, request, arrayElementType, rootKey);

            Expression arrayInit = Expression.NewArrayInit(arrayElementType, arrayExpressionList.Select(e => e.Expression));

            if (request.EnumerableComparer != null)
            {
                arrayInit = CreateSortedArrayExpression(arrayInit, arrayElementType, request);
            }

            var returnResult = request.Services.Compiler.CreateNewResult(request, arrayInit);

            foreach (var result in arrayExpressionList)
            {
                returnResult.AddExpressionResult(result);
            }

            return returnResult;
        }

        /// <summary>
        /// Create an expression to sort the array
        /// </summary>
        /// <param name="arrayInit"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="request"></param>
        protected virtual Expression CreateSortedArrayExpression(Expression arrayInit, Type arrayElementType, IActivationExpressionRequest request)
        {
            var comparerInterface = typeof(IComparer<>).MakeGenericType(arrayElementType);

            if (!comparerInterface.IsAssignableFrom(request.EnumerableComparer.GetType()))
            {
                return arrayInit;
            }

            var sortMethod = typeof(ArrayExpressionCreator)
                .GetMethod(nameof(SortArray))
                .MakeGenericMethod(arrayElementType);

            return Expression.Call(sortMethod, arrayInit, Expression.Constant(request.EnumerableComparer));
        }

        /// <summary>
        /// Get list of expressions to populate array
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="rootKey"></param>
        protected virtual List<IActivationExpressionResult> GetArrayExpressionList(
            IInjectionScope scope, 
            IActivationExpressionRequest request, 
            Type arrayElementType,
            object rootKey)
        {
            var expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, rootKey);

            if (expressions.Count != 0)
            {
                return expressions;
            }

            lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
            {
                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, rootKey);

                if (expressions.Count != 0)
                {
                    return expressions;
                }

                request.Services.Compiler.ProcessMissingStrategyProviders(scope, request.Services.Compiler.CreateNewRequest(arrayElementType, request.ObjectGraphDepth + 1, scope));

                expressions = GetActivationExpressionResultsFromStrategies(scope, request, arrayElementType, rootKey);
            }

            return expressions;
        }

        /// <summary>
        /// Get activation expression for export strategies
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="rootKey">key for keyed Root requests</param>
        protected virtual List<IActivationExpressionResult> GetActivationExpressionResultsFromStrategies(
            IInjectionScope scope,
            IActivationExpressionRequest request,
            Type arrayElementType,
            object rootKey = null)
        {
            // An old odditiy of Grace: 
            // if key is enumerable then it's handled as a collection of keys to be resolved.
            // This isn't implemented everywhere, e.g. LocateAll and Locate<IEnumerable> don't have this behavior.
            if (request.LocateKey is IEnumerable enumerableKey and not string)
            {
                return enumerableKey
                    .Cast<object>()
                    .SelectMany(key =>
                    {
                        var newRequest = request.NewRequest(
                            arrayElementType, 
                            request.RequestingStrategy,
                            request.RequestingStrategy?.ActivationType, 
                            request.RequestType,
                            request.Info, 
                            true, 
                            true);
                        newRequest.SetLocateKey(key);
                        return GetActivationExpressionResultsFromStrategies(scope, newRequest, arrayElementType);
                    })
                    .ToList();
            }

            // Step 1: collect matching strategies
            
            List<ICompiledExportStrategy> exportList = new();

            var key = request.LocateKey ?? rootKey;

            Action<IActivationStrategyCollection<ICompiledExportStrategy>> collect = 
                key != null
                    ? collection => 
                        {
                            if (collection == null) return;                            
                            exportList.AddRange(collection.GetKeyedStrategies(key));
                        }
                    : collection => 
                        {
                            if (collection == null) return;
                            exportList.AddRange(collection.GetStrategies());
                            if (scope.ScopeConfiguration.ReturnKeyedInEnumerable)
                            {
                                exportList.AddRange(collection.GetKeyedStrategies().Select(kvp => kvp.Value));
                            }
                        };

            collect(scope.StrategyCollectionContainer.GetActivationStrategyCollection(arrayElementType));            

            if (arrayElementType.IsConstructedGenericType)
            {
                var genericType = arrayElementType.GetGenericTypeDefinition();
                collect(scope.StrategyCollectionContainer.GetActivationStrategyCollection(genericType));
            }

            exportList.Sort((x, y) => x.Priority != y.Priority 
                ? y.Priority.CompareTo(x.Priority)  // higher priorities first
                : x.ExportOrder.CompareTo(y.ExportOrder));

            // Step 2: create expressions from collected strategies

            var parentStrategy = GetRequestingStrategy(request);
            var expressions = new List<IActivationExpressionResult>();

            foreach (var strategy in exportList)
            {
                // skip as part of the composite pattern
                if (strategy == parentStrategy)
                {
                    continue;
                }

                // filter strategies
                if (request.Filter != null && !request.Filter(strategy))
                {
                    continue;
                }

                var newRequest = request.NewRequest(
                    arrayElementType, 
                    request.RequestingStrategy,
                    request.RequestingStrategy?.ActivationType, 
                    request.RequestType,
                    request.Info, 
                    true, 
                    true);
                newRequest.SetLocateKey(request.LocateKey);

                var expression = strategy.GetActivationExpression(scope, newRequest);
                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }

            if (expressions.Count == 0)
            {
                ProcessWrappers(scope, arrayElementType, request, expressions);
            }

            return expressions;
        }

        private IActivationStrategy GetRequestingStrategy(IActivationExpressionRequest request)
        {
            return request switch
            {
                null => null,
                { RequestingStrategy: { StrategyType: ActivationStrategyType.ExportStrategy } strategy } => strategy,
                _ => GetRequestingStrategy(request.Parent),
            };
        }

        /// <summary>
        /// Process wrappers looking for matching type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="request"></param>
        /// <param name="expressions"></param>
        protected virtual void ProcessWrappers(IInjectionScope scope, Type arrayElementType, IActivationExpressionRequest request, List<IActivationExpressionResult> expressions)
        {
            var wrappers = _wrapperExpressionCreator.GetWrappers(scope, arrayElementType, request, out var wrappedType);

            if (wrappers != ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                wrappers = wrappers.Reverse();

                GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                if (expressions.Count == 0)
                {
                    lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
                    {
                        GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);

                        if (expressions.Count == 0)
                        {
                            var newRequest = request.NewRequest(arrayElementType, request.RequestingStrategy, request.RequestingStrategy?.ActivationType, RequestType.Other, null, true, true);

                            request.Services.Compiler.ProcessMissingStrategyProviders(scope, newRequest);

                            GetExpressionsFromCollections(scope, arrayElementType, request, expressions, wrappedType, wrappers);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort an array using a IComparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayOfT"></param>
        /// <param name="comparer"></param>
        public static T[] SortArray<T>(T[] arrayOfT, IComparer<T> comparer)
        {
            Array.Sort(arrayOfT, comparer);

            return arrayOfT;
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
                    RequestType.Other, null, true, true);

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
