using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Locates and compiles activation strategies for
    /// </summary>
    public class ActivationStrategyCompiler : IActivationStrategyCompiler
    {
        private readonly IInjectionScopeConfiguration _configuration;
        private readonly IActivationExpressionBuilder _builder;
        private readonly IAttributeDiscoveryService _attributeDiscoveryService;
        private readonly IDefaultStrategyExpressionBuilder _exportExpressionBuilder;
        private readonly IInjectionContextCreator _injectionContextCreator;
        private readonly IInjectionStrategyDelegateCreator _injectionStrategyDelegateCreator;
        private readonly IExpressionConstants _constants;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">scope configuration</param>
        /// <param name="builder">activation expression builder</param>
        /// <param name="attributeDiscoveryService">attribute discovery service</param>
        /// <param name="exportExpressionBuilder">expression builder</param>
        /// <param name="injectionContextCreator">injection context creator</param>
        /// <param name="constants">expression constants</param>
        /// <param name="injectionStrategyDelegateCreator">injection strategy creator</param>
        public ActivationStrategyCompiler(IInjectionScopeConfiguration configuration,
                                          IActivationExpressionBuilder builder,
                                          IAttributeDiscoveryService attributeDiscoveryService,
                                          IDefaultStrategyExpressionBuilder exportExpressionBuilder,
                                          IInjectionContextCreator injectionContextCreator,
                                          IExpressionConstants constants,
                                          IInjectionStrategyDelegateCreator injectionStrategyDelegateCreator)
        {
            _configuration = configuration;
            _builder = builder;
            _attributeDiscoveryService = attributeDiscoveryService;
            _constants = constants;
            _injectionStrategyDelegateCreator = injectionStrategyDelegateCreator;
            _exportExpressionBuilder = exportExpressionBuilder;
            _injectionContextCreator = injectionContextCreator;
        }

        /// <summary>
        /// Max object graph depth
        /// </summary>
        public int MaxObjectGraphDepth => _configuration.Behaviors.MaxObjectGraphDepth;

        /// <summary>
        /// Default strategy expression builder
        /// </summary>
        public IDefaultStrategyExpressionBuilder DefaultStrategyExpressionBuilder => _exportExpressionBuilder;

        /// <summary>
        /// Creates a new expression request
        /// </summary>
        /// <param name="activationType">activation type</param>
        /// <param name="objectGraphDepth">current object depth</param>
        /// <param name="requestingScope">requesting scope</param>
        /// <returns>request</returns>
        public virtual IActivationExpressionRequest CreateNewRequest(Type activationType, int objectGraphDepth, IInjectionScope requestingScope)
        {
            if (activationType == null) throw new ArgumentNullException(nameof(activationType));
            if (requestingScope == null) throw new ArgumentNullException(nameof(requestingScope));

            return new ActivationExpressionRequest(
                activationType,
                RequestType.Root,
                new ActivationServices(this, _builder, _attributeDiscoveryService, _exportExpressionBuilder, _injectionContextCreator),
                _constants,
                objectGraphDepth,
                requestingScope,
                new PerDelegateData());
        }

        /// <summary>
        /// Create a new expression result
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        public virtual IActivationExpressionResult CreateNewResult(IActivationExpressionRequest request, Expression expression = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new ActivationExpressionResult(request) { Expression = expression };
        }

        /// <summary>
        /// Find a delegate for a specific type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="consider"></param>
        /// <param name="key"></param>
        /// <param name="forMissingType"></param>
        /// <param name="checkForMissingType"></param>
        public virtual ActivationStrategyDelegate FindDelegate(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key, IInjectionContext forMissingType, bool checkForMissingType)
        {
            var activationDelegate = 
                LocateStrategyFromCollectionContainers(scope, locateType, consider, key, forMissingType)
                ?? LocateEnumerableStrategy(scope, locateType);

            if (activationDelegate != null)
            {
                return activationDelegate;
            }

            if (checkForMissingType)
            {
                lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
                {
                    activationDelegate = LocateStrategyFromCollectionContainers(scope, locateType, consider, key, forMissingType);

                    if (activationDelegate != null)
                    {
                        return activationDelegate;
                    }

                    var request = CreateNewRequest(locateType, 1, scope);

                    request.SetFilter(consider);

                    ProcessMissingStrategyProviders(scope, request);

                    activationDelegate = LocateStrategyFromCollectionContainers(scope, locateType, consider, key, forMissingType);

                    if (activationDelegate != null)
                    {
                        return activationDelegate;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Compile a delegate
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="expressionContext"></param>
        public virtual ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationExpressionResult expressionContext)
        {
            var finalExpression = ProcessExpressionResultForCompile(expressionContext, out var parameters, out var extraExpressions);

            var compiled = CompileExpressionResultToDelegate(expressionContext, parameters, extraExpressions, finalExpression);

            return compiled;
        }


        public T CompileOptimizedDelegate<T>(IInjectionScope scope, IActivationExpressionResult expressionContext)
        {
            var finalExpression = ProcessExpressionResultForCompile(expressionContext, out var parameters, out var extraExpressions);

            var compiled = CompileExpressionResultToOptimized<T>(expressionContext, parameters, extraExpressions, finalExpression);

            return compiled;
        }


        /// <summary>
        /// Create injection delegate 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        public InjectionStrategyDelegate CreateInjectionDelegate(IInjectionScope scope, Type locateType)
        {
            var request = CreateNewRequest(locateType, 1, scope);

            var objectParameter = Expression.Parameter(typeof(object));

            var result =
                _injectionStrategyDelegateCreator.CreateInjectionDelegate(scope, locateType, request, objectParameter);

            if (request.InjectionContextRequired())
            {
                AddInjectionContextExpression(result);
            }

            var methodBlock = Expression.Block(result.ExtraParameters(), result.ExtraExpressions());

            var compiled =
                Expression.Lambda<InjectionStrategyDelegate>(methodBlock,
                        request.Constants.ScopeParameter,
                        request.Constants.RootDisposalScope,
                        request.Constants.InjectionContextParameter,
                        request.Constants.KeyParameter,
                        objectParameter).Compile();

            return compiled;
        }

        /// <summary>
        /// Process expression result for compiling
        /// </summary>
        /// <param name="expressionContext"></param>
        /// <param name="parameters"></param>
        /// <param name="extraExpressions"></param>
        protected virtual Expression ProcessExpressionResultForCompile(IActivationExpressionResult expressionContext,
            out ParameterExpression[] parameters, out Expression[] extraExpressions)
        {
            if (expressionContext.Request.InjectionContextRequired())
            {
                AddInjectionContextExpression(expressionContext);
            }

            var finalExpression = expressionContext.Expression;

            if (!finalExpression.Type.IsByRef)
            {
                finalExpression = Expression.Convert(finalExpression, typeof(object));
            }

            if (finalExpression.NodeType == ExpressionType.Convert &&
               !expressionContext.Expression.Type.GetTypeInfo().IsValueType)
            {
                finalExpression = ((UnaryExpression)finalExpression).Operand;
            }

            parameters = expressionContext.ExtraParameters().ToArray();
            extraExpressions = expressionContext.ExtraExpressions().ToArray();

            return finalExpression;
        }

        /// <summary>
        /// Compiles an expression result to a delegate
        /// </summary>
        /// <param name="expressionContext"></param>
        /// <param name="parameters"></param>
        /// <param name="extraExpressions"></param>
        /// <param name="finalExpression"></param>
        protected virtual ActivationStrategyDelegate CompileExpressionResultToDelegate(
            IActivationExpressionResult expressionContext, ParameterExpression[] parameters, Expression[] extraExpressions,
            Expression finalExpression)
        {
            Expression compileExpression;

            if (parameters.Length == 0 &&
                extraExpressions.Length == 0)
            {
                compileExpression = finalExpression;
            }
            else
            {
                var list = new List<Expression>(expressionContext.ExtraExpressions())
                {
                    finalExpression
                };

                compileExpression = Expression.Block(expressionContext.ExtraParameters(), list);
            }

            return Expression
                .Lambda<ActivationStrategyDelegate>(
                    compileExpression,
                    expressionContext.Request.Constants.ScopeParameter,
                    expressionContext.Request.Constants.RootDisposalScope,
                    expressionContext.Request.Constants.InjectionContextParameter,
                    expressionContext.Request.Constants.KeyParameter
                )
                .Compile();
        }

        protected virtual T CompileExpressionResultToOptimized<T>(
            IActivationExpressionResult expressionContext, ParameterExpression[] parameters, Expression[] extraExpressions,
            Expression finalExpression)
        {
            Expression compileExpression;

            if (parameters.Length == 0 &&
                extraExpressions.Length == 0)
            {
                compileExpression = finalExpression;
            }
            else
            {
                var list = new List<Expression>(expressionContext.ExtraExpressions())
                {
                    finalExpression
                };

                compileExpression = Expression.Block(expressionContext.ExtraParameters(), list);
            }

            var parameterList = new List<ParameterExpression>(4)
            {
                expressionContext.Request.Constants.ScopeParameter,
                expressionContext.Request.Constants.RootDisposalScope,
                expressionContext.Request.Constants.InjectionContextParameter,
                expressionContext.Request.Constants.KeyParameter,
            };

            var invokeMethod = typeof(T).GetTypeInfo().GetDeclaredMethod("Invoke");
            var parameterCount = invokeMethod.GetParameters().Length;

            if (parameterCount > parameterList.Count)
            {
                throw new Exception("Delegate type not supported: " + typeof(T).Name);
            }

            return Expression
                .Lambda<T>(
                    compileExpression, 
                    parameterList.GetRange(0, parameterCount)
                )
                .Compile();
        }

        /// <summary>
        /// Process missing strategy providers
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public void ProcessMissingStrategyProviders(IInjectionScope scope, IActivationExpressionRequest request)
        {
            foreach (var strategyProvider in scope.MissingExportStrategyProviders)
            {
                foreach (var activationStrategy in strategyProvider.ProvideExports(scope, request))
                {
                    switch (activationStrategy)
                    {
                        case ICompiledExportStrategy exportStrategy:
                            scope.StrategyCollectionContainer.AddStrategy(exportStrategy);
                            break;

                        case ICompiledWrapperStrategy wrapperStrategy:
                            scope.WrapperCollectionContainer.AddStrategy(wrapperStrategy);
                            break;

                        case ICompiledDecoratorStrategy decoratorStrategy:
                            scope.DecoratorCollectionContainer.AddStrategy(decoratorStrategy);
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Locate a strategy from collection containers
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="consider"></param>
        /// <param name="key"></param>
        /// <param name="injectionContext"></param>
        protected virtual ActivationStrategyDelegate LocateStrategyFromCollectionContainers(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key, IInjectionContext injectionContext)
        {
            var openGenericType = locateType.IsConstructedGenericType
                ? locateType.GetGenericTypeDefinition() 
                : null;

            if (key != null)
            {
                return FindKeyedStrategy(scope.StrategyCollectionContainer, locateType)
                    ?? FindKeyedStrategy(scope.StrategyCollectionContainer, openGenericType)
                    ?? FindKeyedWrapperStrategy(scope.WrapperCollectionContainer, locateType)
                    ?? FindKeyedWrapperStrategy(scope.WrapperCollectionContainer, openGenericType);
            }
            else
            {
                return FindStrategy(scope.StrategyCollectionContainer, locateType)
                    ?? FindStrategy(scope.StrategyCollectionContainer, openGenericType)
                    ?? FindStrategy(scope.WrapperCollectionContainer, locateType)
                    ?? FindStrategy(scope.WrapperCollectionContainer, openGenericType);
            }

            ActivationStrategyDelegate FindStrategy<T>(
                IActivationStrategyCollectionContainer<T> container,
                Type type)
                where T: IWrapperOrExportActivationStrategy
            {
                if (type == null) return null;

                var collection = container.GetActivationStrategyCollection(type);
                if (collection == null) return null;
                
                var primary = consider == null 
                    ? collection.GetPrimary()?.GetActivationStrategyDelegate(scope, this, locateType)
                    : null;

                return primary ?? GetStrategyFromCollection(collection, scope, consider, locateType);
            }

            ActivationStrategyDelegate FindKeyedWrapperStrategy(
                IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> container,
                Type type)
            {
                if (type == null) return null;

                var collection = container.GetActivationStrategyCollection(type);
                if (collection == null) return null;
                
                var primary = consider == null 
                    ? collection.GetPrimary()?.GetKeyedActivationStrategyDelegate(scope, this, locateType, key)
                    : null;

                return primary ?? GetStrategyFromCollection(collection, scope, consider, locateType, key);
            }            

            ActivationStrategyDelegate FindKeyedStrategy(
                IActivationStrategyCollectionContainer<ICompiledExportStrategy> container,
                Type type)
            {
                if (type == null) return null;

                return container.GetActivationStrategyCollection(type)
                    ?.GetKeyedStrategy(key)
                    ?.GetActivationStrategyDelegate(scope, this, locateType);
            }
        }

        private void AddInjectionContextExpression(IActivationExpressionResult expressionContext)
        {
            var method = typeof(IInjectionContextCreator).GetRuntimeMethod(
                nameof(IInjectionContextCreator.CreateContext),
                new[] { typeof(object) });

            var newExpression = Expression.Call(
                Expression.Constant(_injectionContextCreator),
                method,
                Expression.Constant(null, typeof(object)));

            var assign =
                Expression.Assign(expressionContext.Request.InjectionContextParameter, newExpression);

            var ifThen =
                Expression.IfThen(
                    Expression.Equal(expressionContext.Request.InjectionContextParameter,
                        Expression.Constant(null, typeof(IInjectionContext))),
                    assign);

            expressionContext.AddExtraExpression(ifThen, insertBeginning: true);
        }

        private ActivationStrategyDelegate GetStrategyFromCollection<T>(
            IActivationStrategyCollection<T> strategyCollection, 
            IInjectionScope scope, 
            ActivationStrategyFilter consider, 
            Type locateType,
            object key = null) 
            where T : IWrapperOrExportActivationStrategy
        {
            foreach (var strategy in strategyCollection.GetStrategies())
            {
                if (strategy.HasConditions)
                {
                    foreach (var condition in strategy.Conditions)
                    {
                        if (!condition.MeetsCondition(strategy, new StaticInjectionContext(locateType)))
                        {
                            goto outerLoop;
                        }
                    }
                }

                if (consider?.Invoke(strategy) == false)
                {
                    continue;
                }

                var strategyDelegate = key == null
                    ? strategy.GetActivationStrategyDelegate(scope, this, locateType)
                    : ((ICompiledWrapperStrategy)strategy).GetKeyedActivationStrategyDelegate(scope, this, locateType, key);

                if (strategyDelegate != null)
                {
                    return strategyDelegate;
                }

                outerLoop: continue;
            }

            return null;
        }


        private ActivationStrategyDelegate LocateEnumerableStrategy(IInjectionScope scope, Type locateType)
        {
            if (locateType.IsArray ||
                (locateType.IsConstructedGenericType &&
                 locateType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var result = _builder.GetActivationExpression(scope, CreateNewRequest(locateType, 1, scope));

                return CompileDelegate(scope, result);
            }

            return null;
        }

    }
}
