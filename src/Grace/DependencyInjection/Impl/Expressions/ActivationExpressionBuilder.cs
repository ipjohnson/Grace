using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IActivationExpressionBuilder
    {
        void SetCompiler(IActivationStrategyCompiler compiler);

        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        IActivationExpressionResult GetValueFromRequest(IInjectionScope scope,
                                                        IActivationExpressionRequest request,
                                                        Type activationType,
                                                        object key);

        IActivationExpressionResult DecorateExportStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy);
    }

    public class ActivationExpressionBuilder : IActivationExpressionBuilder
    {
        private readonly IEnumerableExpressionCreator _enumerableExpressionCreator;
        private readonly IArrayExpressionCreator _arrayExpressionCreator;
        private readonly IWrapperExpressionCreator _wrapperExpressionCreator;
        private IActivationStrategyCompiler _compiler;

        public ActivationExpressionBuilder(IArrayExpressionCreator arrayExpressionCreator,
                                           IEnumerableExpressionCreator enumerableExpressionCreator,
                                           IWrapperExpressionCreator wrapperExpressionCreator)
        {
            _enumerableExpressionCreator = enumerableExpressionCreator;
            _arrayExpressionCreator = arrayExpressionCreator;
            _wrapperExpressionCreator = wrapperExpressionCreator;
        }

        public void SetCompiler(IActivationStrategyCompiler compiler)
        {
            _compiler = compiler;
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var activationExpressionResult = GetValueFromRequest(scope, request, request.ActivationType, null);

            if (activationExpressionResult != null)
            {
                return activationExpressionResult;
            }

            activationExpressionResult = GetValueFromInjectionValueProviders(scope, request);

            if (activationExpressionResult != null)
            {
                return activationExpressionResult;
            }

            activationExpressionResult = GetActivationExpressionFromStrategies(scope, request);

            if (activationExpressionResult != null)
            {
                return activationExpressionResult;
            }

            if (request.ActivationType.IsArray)
            {
                return _arrayExpressionCreator.GetArrayExpression(scope, request);
            }

            if (request.ActivationType.IsConstructedGenericType &&
                request.ActivationType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return _enumerableExpressionCreator.GetEnumerableExpression(scope, request, _arrayExpressionCreator);
            }

            var wrapperResult = _wrapperExpressionCreator.GetActivationStrategy(scope, request);

            if (wrapperResult != null)
            {
                return wrapperResult;
            }

            if (scope.MissingExportStrategyProviders.Any())
            {
                lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
                {
                    activationExpressionResult = GetActivationExpressionFromStrategies(scope, request);

                    if (activationExpressionResult != null)
                    {
                        return activationExpressionResult;
                    }

                    wrapperResult = _wrapperExpressionCreator.GetActivationStrategy(scope, request);

                    if (wrapperResult != null)
                    {
                        return wrapperResult;
                    }

                    request.Services.Compiler.ProcessMissingStrategyProviders(scope, request);

                    activationExpressionResult = GetActivationExpressionFromStrategies(scope, request);

                    if (activationExpressionResult != null)
                    {
                        return activationExpressionResult;
                    }

                    wrapperResult = _wrapperExpressionCreator.GetActivationStrategy(scope, request);

                    if (wrapperResult != null)
                    {
                        return wrapperResult;
                    }
                }
            }

            var parent = scope.Parent as IInjectionScope;

            if (parent != null)
            {
                return GetActivationExpression(parent, request);
            }

            return GetValueFromInjectionContext(scope, request);
        }

        private IActivationExpressionResult GetValueFromInjectionValueProviders(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (!ReferenceEquals(scope.InjectionValueProviders, ImmutableLinkedList<IInjectionValueProvider>.Empty))
            {
                foreach (var valueProvider in scope.InjectionValueProviders)
                {
                    var result = valueProvider.GetExpressionResult(scope, request);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            var parent = scope.Parent as IInjectionScope;

            return parent != null ? GetValueFromInjectionValueProviders(parent, request) : null;
        }

        public IActivationExpressionResult GetValueFromRequest(IInjectionScope scope,
                                                               IActivationExpressionRequest request,
                                                               Type activationType,
                                                               object key)
        {
            var knownValue =
                request.KnownValueExpressions.FirstOrDefault(
                    v => activationType.GetTypeInfo().IsAssignableFrom(v.ActivationType.GetTypeInfo()));

            if (knownValue != null)
            {
                return knownValue.ValueExpression(request);
            }

            if (request.WrapperPathNode != null)
            {
                if (activationType.GetTypeInfo().IsAssignableFrom(request.WrapperPathNode.ActivationType.GetTypeInfo()))
                {
                    var wrapper = request.PopWrapperPathNode();

                    return ProcessPathNode(scope, request, activationType, wrapper);
                }
            }
            else if (request.DecoratorPathNode != null)
            {
                if (activationType.GetTypeInfo().IsAssignableFrom(request.DecoratorPathNode.Strategy.ActivationType.GetTypeInfo()))
                {
                    var decorator = request.PopDecoratorPathNode();

                    return ProcessPathNode(scope, request, activationType, decorator);
                }
            }

            if (request.ActivationType == typeof(IInjectionScope))
            {
                if (!scope.ScopeConfiguration.Behaviors.AllowInjectionScopeLocation)
                {
                    throw new ImportInjectionScopeException(request.GetStaticInjectionContext());
                }

                var method = typeof(IExportLocatorScopeExtensions).GetRuntimeMethod("GetInjectionScope", new[] { typeof(IExportLocatorScope) });

                var expression = Expression.Call(method, request.Constants.ScopeParameter);

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            if (request.ActivationType == typeof(IExportLocatorScope) ||
                request.ActivationType == typeof(ILocatorService))
            {
                return request.Services.Compiler.CreateNewResult(request, request.Constants.ScopeParameter);
            }

            if (request.ActivationType == typeof(IInjectionContext))
            {
                request.RequireInjectionContext();

                return request.Services.Compiler.CreateNewResult(request, request.Constants.InjectionContextParameter);
            }

            if (request.ActivationType == typeof(StaticInjectionContext))
            {
                var staticContext = request.GetStaticInjectionContext();

                return request.Services.Compiler.CreateNewResult(request, Expression.Constant(staticContext));
            }

            if (request.IsDynamic)
            {
                var dynamicMethod =
                    typeof(ActivationExpressionBuilder).GetRuntimeMethod("GetDynamicValue",
                        new[]
                        {
                            typeof(IExportLocatorScope),
                            typeof(IDisposalScope),
                            typeof(StaticInjectionContext),
                            typeof(IInjectionContext),
                            typeof(object),
                            typeof(bool)
                        });

                var closedMethod = dynamicMethod.MakeGenericMethod(request.ActivationType);

                var expression = Expression.Call(closedMethod,
                                                 request.Constants.ScopeParameter,
                                                 request.DisposalScopeExpression,
                                                 Expression.Constant(request.GetStaticInjectionContext()),
                                                 request.Constants.InjectionContextParameter,
                                                 Expression.Constant(request.LocateKey, typeof(object)),
                                                 Expression.Constant(request.IsRequired));

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            return null;
        }

        /// <summary>
        /// Get a value dynamically
        /// </summary>
        /// <typeparam name="T">value to get</typeparam>
        /// <param name="scope">scope</param>
        /// <param name="disposalScope">disposal scope to use</param>
        /// <param name="staticInjectionContext">static injection context </param>
        /// <param name="context">context for call</param>
        /// <param name="key"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static T GetDynamicValue<T>(IExportLocatorScope scope, IDisposalScope disposalScope, StaticInjectionContext staticInjectionContext,
            IInjectionContext context, object key, bool isRequired)
        {
            var injectionScope = scope.GetInjectionScope();

            var value = injectionScope.LocateFromChildScope(scope, disposalScope, typeof(T), context, null, key, true, true);

            if (value == null)
            {
                if (isRequired)
                {
                    throw new LocateException(staticInjectionContext, $"Could not locate dynamic value for type {typeof(T).FullName}");
                }

                return default(T);
            }

            return (T)value;
        }

        private IActivationExpressionResult ProcessPathNode(IInjectionScope scope, IActivationExpressionRequest request, Type activationType, IActivationPathNode decorator)
        {
            return decorator.GetActivationExpression(scope, request);
        }

        private IActivationExpressionResult GetActivationExpressionFromStrategies(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var expressionResult = GetExpressionFromStrategyCollection(scope, request);

            if (expressionResult != null)
            {
                return expressionResult;
            }

            return GetExpressionFromGenericStrategies(scope, request);
        }

        private IActivationExpressionResult GetExpressionFromGenericStrategies(IInjectionScope scope, IActivationExpressionRequest request)
        {
            if (request.ActivationType.IsConstructedGenericType)
            {
                var genericType = request.ActivationType.GetGenericTypeDefinition();

                var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(genericType);

                if (collection != null)
                {
                    if (request.LocateKey != null)
                    {
                        var keyedStrategy = collection.GetKeyedStrategy(request.LocateKey);

                        if (keyedStrategy != null)
                        {
                            return ActivationExpressionForStrategy(scope, request, keyedStrategy);
                        }
                    }
                    else
                    {
                        var strategy = collection.GetPrimary();

                        if (strategy != null && request.Filter == null)
                        {
                            return ActivationExpressionForStrategy(scope, request, strategy);
                        }

                        strategy = SelectStrategyFromCollection(collection, scope, request);

                        if (strategy != null)
                        {
                            return ActivationExpressionForStrategy(scope, request, strategy);
                        }
                    }
                }
            }

            return null;
        }

        private ICompiledExportStrategy SelectStrategyFromCollection(IActivationStrategyCollection<ICompiledExportStrategy> collection, IInjectionScope scope, IActivationExpressionRequest request)
        {
            var filter = request.Filter;

            foreach (var strategy in collection.GetStrategies())
            {
                if (filter != null && !filter(strategy))
                {
                    continue;
                }

                if (!strategy.HasConditions)
                {
                    return strategy;
                }

                var context = request.GetStaticInjectionContext();

                if (!strategy.Conditions.All(condition => condition.MeetsCondition(strategy, context)))
                {
                    continue;
                }

                return strategy;
            }

            return null;
        }

        private IActivationExpressionResult GetExpressionFromStrategyCollection(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(request.ActivationType);

            if (collection != null)
            {
                if (request.LocateKey != null)
                {
                    var keyedStrategy = collection.GetKeyedStrategy(request.LocateKey);

                    if (keyedStrategy != null)
                    {
                        return ActivationExpressionForStrategy(scope, request, keyedStrategy);
                    }
                }
                else
                {
                    var strategy =  request.Filter == null ? collection.GetPrimary() : null;

                    if (strategy != null)
                    {
                        return ActivationExpressionForStrategy(scope, request, strategy);
                    }

                    strategy = SelectStrategyFromCollection(collection, scope, request);

                    if (strategy != null)
                    {
                        return ActivationExpressionForStrategy(scope, request, strategy);
                    }
                }
            }

            return null;
        }

        private IActivationExpressionResult ActivationExpressionForStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy)
        {
            return strategy.GetActivationExpression(scope, request);
        }

        public IActivationExpressionResult GetValueFromInjectionContext(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var valueMethod = typeof(ActivationExpressionBuilder).GetRuntimeMethod("GetValueFromInjectionContext", new Type[]
            {
                typeof(IExportLocatorScope),
                typeof(StaticInjectionContext),
                typeof(object),
                typeof(IInjectionContext),
                typeof(object),
                typeof(bool),
                typeof(bool)
            });

            var closedMethod = valueMethod.MakeGenericMethod(request.ActivationType);

            var expresion = Expression.Call(closedMethod,
                                            request.Constants.ScopeParameter,
                                            Expression.Constant(request.GetStaticInjectionContext()),
                                            Expression.Constant(request.LocateKey, typeof(object)),
                                            request.Constants.InjectionContextParameter,
                                            Expression.Constant(request.DefaultValue?.DefaultValue, typeof(object)),
                                            Expression.Constant(request.DefaultValue != null),
                                            Expression.Constant(request.IsRequired));

            return request.Services.Compiler.CreateNewResult(request, expresion);
        }

        public static T GetValueFromInjectionContext<T>(
            IExportLocatorScope locator,
            StaticInjectionContext staticContext,
            object key,
            IInjectionContext dataProvider,
            object defaultValue,
            bool useDefault,
            bool isRequired)
        {
            object value = null;

            if (dataProvider != null && key != null)
            {
                value = dataProvider.GetExtraData(key);
            }

            if (value == null && useDefault)
            {
                var defaultFunc = defaultValue as Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, T>;

                value = defaultFunc != null ? defaultFunc(locator, staticContext, dataProvider) : defaultValue;
            }

            if (value != null)
            {
                if (!value.GetType().GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
                {
                    try
                    {
                        value = Convert.ChangeType(value, typeof(T));
                    }
                    catch (Exception exp)
                    {
                        // to do fix up exception
                        throw new LocateException(staticContext, exp);
                    }
                }
            }
            else if (isRequired && !useDefault)
            {
                throw new LocateException(staticContext);
            }

            return (T)value;
        }

        public IActivationExpressionResult DecorateExportStrategy(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledExportStrategy strategy)
        {
            var decorators = FindDecoratorsForStrategy(scope, request);

            if (decorators.Count == 0)
            {
                return null;
            }

            return CreateDecoratedActiationStrategy(scope, request, strategy, decorators);
        }

        protected virtual List<ICompiledDecoratorStrategy> FindDecoratorsForStrategy(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var decorators = new List<ICompiledDecoratorStrategy>();

            var collection =
                scope.DecoratorCollectionContainer.GetActivationStrategyCollection(request.ActivationType);

            if (collection != null)
            {
                decorators.AddRange(collection.GetStrategies());
            }

            if (request.ActivationType.IsConstructedGenericType)
            {
                var generic = request.ActivationType.GetGenericTypeDefinition();

                collection = scope.DecoratorCollectionContainer.GetActivationStrategyCollection(generic);

                if (collection != null)
                {
                    decorators.AddRange(collection.GetStrategies());
                }
            }

            return decorators;
        }

        protected virtual IActivationExpressionResult CreateDecoratedActiationStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy, List<ICompiledDecoratorStrategy> decorators)
        {
            decorators.Sort((x, y) => Comparer<int>.Default.Compare(x.Priority, y.Priority));

            ImmutableLinkedList<IActivationPathNode> pathNodes = ImmutableLinkedList<IActivationPathNode>.Empty;

            if (decorators.All(d => d.ApplyAfterLifestyle))
            {
                pathNodes = pathNodes.Add(new DecoratorActivationPathNode(strategy, request.ActivationType, strategy.Lifestyle));

                foreach (var decorator in decorators)
                {
                    pathNodes = pathNodes.Add(new DecoratorActivationPathNode(decorator, request.ActivationType, null));
                }
            }
            else
            {
                pathNodes = pathNodes.Add(new DecoratorActivationPathNode(strategy, request.ActivationType, null));

                DecoratorActivationPathNode currentNode = null;

                foreach (var decorator in decorators.Where(d => !d.ApplyAfterLifestyle))
                {
                    currentNode = new DecoratorActivationPathNode(decorator, request.ActivationType, null);

                    pathNodes = pathNodes.Add(currentNode);
                }

                if (currentNode != null)
                {
                    currentNode.Lifestyle = strategy.Lifestyle;
                }

                foreach (var decorator in decorators.Where(d => d.ApplyAfterLifestyle))
                {
                    pathNodes = pathNodes.Add(new DecoratorActivationPathNode(decorator, request.ActivationType, null));
                }
            }

            request.SetDecoratorPath(pathNodes);

            var pathNode = request.PopDecoratorPathNode();

            return pathNode.GetActivationExpression(scope, request);
        }
    }
}

