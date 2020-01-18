using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// interface for building Linq Expression tree for strategy activation
    /// </summary>
    public interface IActivationExpressionBuilder
    {
        /// <summary>
        /// Get a linq expression to satisfy the request
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <returns></returns>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Decorate an export strategy with decorators
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <param name="strategy">strategy being decorated</param>
        /// <returns></returns>
        IActivationExpressionResult DecorateExportStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy);
    }

    /// <summary>
    /// builder creates linq expression to satify request
    /// </summary>
    public class ActivationExpressionBuilder : IActivationExpressionBuilder
    {
        /// <summary>
        /// Enumerable expression creator
        /// </summary>
        protected readonly IEnumerableExpressionCreator EnumerableExpressionCreator;

        /// <summary>
        /// Array expression creator
        /// </summary>
        protected readonly IArrayExpressionCreator ArrayExpressionCreator;

        /// <summary>
        /// Wrapper expression creator
        /// </summary>
        protected readonly IWrapperExpressionCreator WrapperExpressionCreator;

        private readonly IInjectionContextValueProvider _contextValueProvider;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="arrayExpressionCreator"></param>
        /// <param name="enumerableExpressionCreator"></param>
        /// <param name="wrapperExpressionCreator"></param>
        /// <param name="contextValueProvider"></param>
        public ActivationExpressionBuilder(IArrayExpressionCreator arrayExpressionCreator,
                                           IEnumerableExpressionCreator enumerableExpressionCreator,
                                           IWrapperExpressionCreator wrapperExpressionCreator,
                                           IInjectionContextValueProvider contextValueProvider)
        {
            EnumerableExpressionCreator = enumerableExpressionCreator;
            ArrayExpressionCreator = arrayExpressionCreator;
            WrapperExpressionCreator = wrapperExpressionCreator;
            _contextValueProvider = contextValueProvider;
        }

        /// <summary>
        /// Get a linq expression to satisfy the request
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <returns></returns>
        public virtual IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var activationExpressionResult = GetValueFromRequest(scope, request, request.ActivationType, request.LocateKey);

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
                return ArrayExpressionCreator.GetArrayExpression(scope, request);
            }

            if (request.ActivationType.IsConstructedGenericType &&
                request.ActivationType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return EnumerableExpressionCreator.GetEnumerableExpression(scope, request, ArrayExpressionCreator);
            }

            var wrapperResult = WrapperExpressionCreator.GetActivationExpression(scope, request);

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

                    wrapperResult = WrapperExpressionCreator.GetActivationExpression(scope, request);

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

                    wrapperResult = WrapperExpressionCreator.GetActivationExpression(scope, request);

                    if (wrapperResult != null)
                    {
                        return wrapperResult;
                    }
                }
            }

            foreach (var scopeMissingDependencyExpressionProvider in scope.MissingDependencyExpressionProviders)
            {
                var result = scopeMissingDependencyExpressionProvider.ProvideExpression(scope, request);

                if (result != null)
                {
                    return result;
                }
            }

            if (scope.Parent is IInjectionScope parent)
            {
                return GetActivationExpression(parent, request);
            }

            return GetValueFromInjectionContext(scope, request);
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
        /// <param name="hasDefault"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetDynamicValue<T>(IExportLocatorScope scope, IDisposalScope disposalScope, StaticInjectionContext staticInjectionContext,
            IInjectionContext context, object key, bool isRequired, bool hasDefault, object defaultValue)
        {
            var injectionScope = scope.GetInjectionScope();

            var value = injectionScope.LocateFromChildScope(scope, disposalScope, typeof(T), context, null, key, true, true);

            if (value != null)
            {
                return (T)value;
            }

            if (hasDefault)
            {
                return (T)defaultValue;
            }

            if (isRequired)
            {
                throw new LocateException(staticInjectionContext, $"Could not locate dynamic value for type {typeof(T).FullName}");
            }

            return default(T);
        }

        /// <summary>
        /// Creates expression for calling method GetValueFromInjectionContext
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual IActivationExpressionResult GetValueFromInjectionContext(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var valueMethod = _contextValueProvider.GetType().GetRuntimeMethod(nameof(GetValueFromInjectionContext), new[]
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

            var key = request.LocateKey;

            if (key is string)
            {
                key = ((string)key).ToLowerInvariant();
            }

            request.RequireExportScope();
            
            var expression = Expression.Call(Expression.Constant(_contextValueProvider),
                                            closedMethod,
                                            request.ScopeParameter,
                                            Expression.Constant(request.GetStaticInjectionContext()),
                                            Expression.Constant(key, typeof(object)),
                                            request.InjectionContextParameter,
                                            Expression.Constant(request.DefaultValue?.DefaultValue, typeof(object)),
                                            Expression.Constant(request.DefaultValue != null),
                                            Expression.Constant(request.IsRequired));

            var result = request.Services.Compiler.CreateNewResult(request, expression);

            result.UsingFallbackExpression = true;

            return result;
        }

        /// <summary>
        /// Decorate an export strategy with decorators
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request</param>
        /// <param name="strategy">strategy being decorated</param>
        /// <returns></returns>
        public IActivationExpressionResult DecorateExportStrategy(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledExportStrategy strategy)
        {
            var decorators = FindDecoratorsForStrategy(scope, request, strategy);

            return decorators.Count != 0 ? CreateDecoratedActivationStrategy(scope, request, strategy, decorators) : null;
        }

        #region protected methods

        /// <summary>
        /// Get IInjectionValueProviders for expression result for request
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetValueFromInjectionValueProviders(IInjectionScope scope, IActivationExpressionRequest request)
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

        /// <summary>
        /// Get expression result from request
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="activationType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetValueFromRequest(IInjectionScope scope,
                                                               IActivationExpressionRequest request,
                                                               Type activationType,
                                                               object key)
        {
            var knownValues =
                request.KnownValueExpressions.Where(
                    v => activationType.GetTypeInfo().IsAssignableFrom(v.ActivationType.GetTypeInfo())).ToArray();

            if (knownValues.Length > 0)
            {
                if (knownValues.Length == 1)
                {
                    return knownValues[0].ValueExpression(request);
                }

                if (key != null)
                {
                    IKnownValueExpression knownValue;

                    if (key is string keyString)
                    {
                        knownValue =
                            knownValues.FirstOrDefault(v =>
                                string.Compare(keyString, v.Key as string, StringComparison.CurrentCultureIgnoreCase) == 0);
                    }
                    else
                    {
                        knownValue = knownValues.FirstOrDefault(v => v.Key == key);
                    }

                    if (knownValue != null)
                    {
                        return knownValue.ValueExpression(request);
                    }
                }

                if (request.Info is MemberInfo memberInfo)
                {
                    var knownValue = knownValues.FirstOrDefault(v => Equals(v.Key, memberInfo.Name));

                    if (knownValue != null)
                    {
                        return knownValue.ValueExpression(request);
                    }
                }

                if (request.Info is ParameterInfo parameterInfo)
                {
                    var knownValue = knownValues.FirstOrDefault(v => Equals(v.Key, parameterInfo.Name));

                    if (knownValue != null)
                    {
                        return knownValue.ValueExpression(request);
                    }

                    knownValue = knownValues.FirstOrDefault(v =>
                        Equals(v.Position.GetValueOrDefault(-1), parameterInfo.Position));

                    if (knownValue != null)
                    {
                        return knownValue.ValueExpression(request);
                    }
                }

                return knownValues[0].ValueExpression(request);
            }

            if (request.WrapperPathNode != null)
            {
                var configuration = request.WrapperPathNode.Strategy.GetActivationConfiguration(activationType);

                if (configuration.ActivationType != null &&
                    (activationType.GetTypeInfo().IsAssignableFrom(configuration.ActivationType.GetTypeInfo()) || 
                     activationType.IsConstructedGenericType && activationType.GetGenericTypeDefinition() == configuration.ActivationType))
                {
                    var wrapper = request.PopWrapperPathNode();

                    return ProcessPathNode(scope, request, activationType, wrapper);
                }
            }
            else if (request.DecoratorPathNode != null)
            {
                var configuration = request.DecoratorPathNode.Strategy.GetActivationConfiguration(activationType);

                if (configuration.ActivationType != null &&
                    (activationType.GetTypeInfo().IsAssignableFrom(configuration.ActivationType.GetTypeInfo()) ||
                     activationType.IsConstructedGenericType && activationType.GetGenericTypeDefinition() == configuration.ActivationType))
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

                request.RequireExportScope();

                var method = typeof(IExportLocatorScopeExtensions).GetRuntimeMethod(nameof(IExportLocatorScopeExtensions.GetInjectionScope), new[] { typeof(IExportLocatorScope) });

                var expression = Expression.Call(method, request.ScopeParameter);

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            if (request.ActivationType == typeof(IExportLocatorScope) ||
                request.ActivationType == typeof(ILocatorService))
            {
                request.RequireExportScope();

                return request.Services.Compiler.CreateNewResult(request, request.ScopeParameter);
            }

            if (request.ActivationType == typeof(IDisposalScope) ||
                (request.ActivationType == typeof(IDisposable) &&
                 request.RequestingScope.ScopeConfiguration.InjectIDisposable))
            {
                request.RequireDisposalScope();

                return request.Services.Compiler.CreateNewResult(request, request.DisposalScopeExpression);
            }

            if (request.ActivationType == typeof(IInjectionContext))
            {
                request.RequireInjectionContext();

                return request.Services.Compiler.CreateNewResult(request, request.InjectionContextParameter);
            }

            if (request.ActivationType == typeof(StaticInjectionContext))
            {
                var staticContext = request.Parent != null ?
                                    request.Parent.GetStaticInjectionContext() :
                                    request.GetStaticInjectionContext();

                return request.Services.Compiler.CreateNewResult(request, Expression.Constant(staticContext));
            }

            if (request.IsDynamic)
            {
                var dynamicMethod =
                    typeof(ActivationExpressionBuilder).GetRuntimeMethod(nameof(GetDynamicValue),
                        new[]
                        {
                            typeof(IExportLocatorScope),
                            typeof(IDisposalScope),
                            typeof(StaticInjectionContext),
                            typeof(IInjectionContext),
                            typeof(object),
                            typeof(bool),
                            typeof(bool),
                            typeof(object)
                        });

                var closedMethod = dynamicMethod.MakeGenericMethod(request.ActivationType);

                Expression defaultExpression =
                    Expression.Constant(request.DefaultValue?.DefaultValue, typeof(object));

                request.RequireExportScope();
                request.RequireDisposalScope();
                request.RequireInjectionContext();

                var expression = Expression.Call(closedMethod,
                                                 request.ScopeParameter,
                                                 request.DisposalScopeExpression,
                                                 Expression.Constant(request.GetStaticInjectionContext()),
                                                 request.InjectionContextParameter,
                                                 Expression.Constant(request.LocateKey, typeof(object)),
                                                 Expression.Constant(request.IsRequired),
                                                 Expression.Constant(request.DefaultValue != null),
                                                 defaultExpression);

                return request.Services.Compiler.CreateNewResult(request, expression);
            }

            return null;
        }

        /// <summary>
        /// Get expression from decorator
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="activationType"></param>
        /// <param name="decorator"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult ProcessPathNode(IInjectionScope scope, IActivationExpressionRequest request, Type activationType, IActivationPathNode decorator)
        {
            return decorator.GetActivationExpression(scope, request);
        }

        /// <summary>
        /// Get expression for non generic strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetActivationExpressionFromStrategies(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var expressionResult = GetExpressionFromStrategyCollection(scope, request);

            if (expressionResult != null)
            {
                return expressionResult;
            }

            return GetExpressionFromGenericStrategies(scope, request);
        }

        /// <summary>
        /// Get expression for generic strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetExpressionFromGenericStrategies(IInjectionScope scope, IActivationExpressionRequest request)
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
                            var result = ActivationExpressionForStrategy(scope, request, strategy);

                            if (result != null)
                            {
                                return result;
                            }
                        }

                        return SelectStrategyFromCollection(collection, scope, request);
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Get expression from strategy collections (export and wrapper)
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult GetExpressionFromStrategyCollection(IInjectionScope scope, IActivationExpressionRequest request)
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
                    var strategy = request.Filter == null ? collection.GetPrimary() : null;

                    if (strategy != null &&
                        strategy != request.RequestingStrategy)
                    {
                        var result = ActivationExpressionForStrategy(scope, request, strategy);

                        if (result != null)
                        {
                            return result;
                        }
                    }

                    return SelectStrategyFromCollection(collection, scope, request);
                }
            }

            return null;
        }

        /// <summary>
        /// Select the best strategy from collection to satisfy request
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult SelectStrategyFromCollection(IActivationStrategyCollection<ICompiledExportStrategy> collection, IInjectionScope scope, IActivationExpressionRequest request)
        {
            var filter = request.Filter;
            IActivationExpressionResult result = null;

            foreach (var strategy in collection.GetStrategies())
            {
                if (filter != null && !filter(strategy))
                {
                    continue;
                }

                if (strategy.HasConditions)
                {
                    var context = request.GetStaticInjectionContext();

                    if (!strategy.Conditions.All(condition => condition.MeetsCondition(strategy, context)))
                    {
                        continue;
                    }
                }

                if (request.RequestingStrategy == strategy)
                {
                    continue;
                }

                result = strategy.GetActivationExpression(scope, request);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an activation expression for a given strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult ActivationExpressionForStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy)
        {
            return strategy.GetActivationExpression(scope, request);
        }

        /// <summary>
        /// Finds decorators for a strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        protected virtual List<ICompiledDecoratorStrategy> FindDecoratorsForStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy)
        {
            var decorators = FindDecoratorsForType(scope, request, request.ActivationType, strategy);

            if (request.ActivationType != strategy.ActivationType)
            {
                var activationTypeDecorators = FindDecoratorsForType(scope, request, strategy.ActivationType, strategy);

                foreach (var decorator in activationTypeDecorators)
                {
                    if (decorators.Contains(decorator))
                    {
                        continue;
                    }

                    decorators.Add(decorator);
                }
            }

            return decorators;
        }

        /// <summary>
        /// Find decorators for a given type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="type"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        protected virtual List<ICompiledDecoratorStrategy> FindDecoratorsForType(IInjectionScope scope, IActivationExpressionRequest request, Type type, ICompiledExportStrategy strategy)
        {
            List<ICompiledDecoratorStrategy> decorators = new List<ICompiledDecoratorStrategy>();
            StaticInjectionContext staticInjectionContext = null;
            var collection =
                scope.DecoratorCollectionContainer.GetActivationStrategyCollection(type);

            if (collection != null)
            {
                foreach (var decorator in collection.GetStrategies())
                {
                    if (decorator.HasConditions)
                    {
                        if (staticInjectionContext == null)
                        {
                            staticInjectionContext = request.GetStaticInjectionContext();
                        }

                        if (!decorator.Conditions.All(
                                condition => condition.MeetsCondition(strategy, staticInjectionContext)))
                        {
                            continue;
                        }
                    }

                    decorators.Add(decorator);
                }
            }

            if (type.IsConstructedGenericType)
            {
                var generic = type.GetGenericTypeDefinition();

                collection = scope.DecoratorCollectionContainer.GetActivationStrategyCollection(generic);

                if (collection != null)
                {
                    foreach (var decorator in collection.GetStrategies())
                    {
                        if (decorator.HasConditions)
                        {
                            if (staticInjectionContext == null)
                            {
                                staticInjectionContext = request.GetStaticInjectionContext();
                            }

                            if (!decorator.Conditions.All(
                                    condition => condition.MeetsCondition(strategy, staticInjectionContext)))
                            {
                                continue;
                            }
                        }

                        decorators.Add(decorator);
                    }
                }
            }

            return decorators;
        }

        /// <summary>
        /// Creates decorated expression for activation strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="strategy"></param>
        /// <param name="decorators"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateDecoratedActivationStrategy(IInjectionScope scope, IActivationExpressionRequest request, ICompiledExportStrategy strategy, List<ICompiledDecoratorStrategy> decorators)
        {
            decorators.Sort((x, y) => Comparer<int>.Default.Compare(x.Priority, y.Priority));

            var pathNodes = ImmutableLinkedList<IActivationPathNode>.Empty;

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
        #endregion
    }
}



