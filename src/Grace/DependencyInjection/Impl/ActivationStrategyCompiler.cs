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

            return new ActivationExpressionRequest(activationType,
                                                   RequestType.Root,
                                                   new ActivationServices(this, _builder, _attributeDiscoveryService, _exportExpressionBuilder, _injectionContextCreator),
                                                   _constants,
                                                   objectGraphDepth,
                                                   requestingScope,
                                                   new PerDelegateData());
        }

        /// <summary>
        /// Create a new expresion result
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
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
        /// <param name="injectionContext"></param>
        /// <param name="checkMissing"></param>
        /// <returns></returns>
        public virtual ActivationStrategyDelegate FindDelegate(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key, IInjectionContext injectionContext, bool checkMissing)
        {
            var activationDelegate = LocateStrategyFromCollectionContainers(scope, locateType, consider, key, injectionContext);

            if (activationDelegate != null)
            {
                return activationDelegate;
            }

            activationDelegate = LocateEnumerableStrategy(scope, locateType, consider, key);

            if (activationDelegate != null)
            {
                return activationDelegate;
            }

            if (checkMissing)
            {
                lock (scope.GetLockObject(InjectionScope.ActivationStrategyAddLockName))
                {
                    activationDelegate = LocateStrategyFromCollectionContainers(scope, locateType, consider, key, injectionContext);

                    if (activationDelegate != null)
                    {
                        return activationDelegate;
                    }

                    var request = CreateNewRequest(locateType, 1, scope);

                    request.SetFilter(consider);

                    ProcessMissingStrategyProviders(scope, request);

                    activationDelegate = LocateStrategyFromCollectionContainers(scope, locateType, consider, key, injectionContext);

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
        /// <returns></returns>
        public virtual ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationExpressionResult expressionContext)
        {
            ParameterExpression[] parameters;
            Expression[] extraExpressions;

            var finalExpression = ProcessExpressionResultForCompile(expressionContext, out parameters, out extraExpressions);

            var compiled = CompileExpressionResultToDelegate(expressionContext, parameters, extraExpressions, finalExpression);

            return compiled;
        }


        public T CompileOptimizedDelegate<T>(IInjectionScope scope, IActivationExpressionResult expressionContext)
        {
            ParameterExpression[] parameters;
            Expression[] extraExpressions;

            var finalExpression = ProcessExpressionResultForCompile(expressionContext, out parameters, out extraExpressions);

            var compiled = CompileExpressionResultToOpitimzed<T>(expressionContext, parameters, extraExpressions, finalExpression);

            return compiled;
        }


        /// <summary>
        /// Create injection delegate 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <returns></returns>
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
                        objectParameter).Compile();

            return compiled;
        }

        /// <summary>
        /// Process expression result for compiling
        /// </summary>
        /// <param name="expressionContext"></param>
        /// <param name="parameters"></param>
        /// <param name="extraExpressions"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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

            var compiled =
                Expression.Lambda<ActivationStrategyDelegate>(compileExpression,
                        expressionContext.Request.Constants.ScopeParameter,
                        expressionContext.Request.Constants.RootDisposalScope,
                        expressionContext.Request.Constants.InjectionContextParameter).Compile();

            return compiled;
        }


        //TODO: Typo in method name.
        protected virtual T CompileExpressionResultToOpitimzed<T>(
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

            var parameterList = new List<ParameterExpression>();

            var invokeMethod = typeof(T).GetTypeInfo().GetDeclaredMethod("Invoke");
            var parameterInfos = invokeMethod.GetParameters();

            if (parameterInfos.Length > 3)
            {
                throw new Exception("Delegate type not supported: " + typeof(T).Name);
            }

            if (parameterInfos.Length > 0)
            {
                parameterList.Add(expressionContext.Request.Constants.ScopeParameter);
            }

            if (parameterInfos.Length > 1)
            {
                parameterList.Add(expressionContext.Request.Constants.RootDisposalScope);
            }

            if (parameterInfos.Length > 2)
            {
                parameterList.Add(expressionContext.Request.Constants.InjectionContextParameter);
            }

            var compiled =
                Expression.Lambda<T>(compileExpression, parameterList).Compile();

            return compiled;
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
                    if (activationStrategy is ICompiledExportStrategy)
                    {
                        scope.StrategyCollectionContainer.AddStrategy(activationStrategy as ICompiledExportStrategy);
                    }
                    else if (activationStrategy is ICompiledWrapperStrategy)
                    {
                        scope.WrapperCollectionContainer.AddStrategy(activationStrategy as ICompiledWrapperStrategy);
                    }
                    else if (activationStrategy is ICompiledDecoratorStrategy)
                    {
                        scope.DecoratorCollectionContainer.AddStrategy(activationStrategy as ICompiledDecoratorStrategy);
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
        /// <returns></returns>
        protected virtual ActivationStrategyDelegate LocateStrategyFromCollectionContainers(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key, IInjectionContext injectionContext)
        {
            if (key != null)
            {
                return FindKeyedDelegate(scope, locateType, consider, key);
            }

            var strategyCollection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(locateType);

            if (strategyCollection != null)
            {
                var primary = consider == null ?
                    strategyCollection.GetPrimary()?.GetActivationStrategyDelegate(scope, this, locateType) : null;

                if (primary != null)
                {
                    return primary;
                }

                var strategyDelegate = GetStrategyFromCollection(strategyCollection, scope, consider, locateType, injectionContext);

                if (strategyDelegate != null)
                {
                    return strategyDelegate;
                }
            }

            var isGeneric = locateType.IsConstructedGenericType;

            if (isGeneric)
            {
                var generic = locateType.GetGenericTypeDefinition();

                strategyCollection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(generic);

                if (strategyCollection != null)
                {
                    var primary = consider == null ?
                        strategyCollection.GetPrimary()?.GetActivationStrategyDelegate(scope, this, locateType) : null;

                    if (primary != null)
                    {
                        return primary;
                    }

                    var strategyDelegate = GetStrategyFromCollection(strategyCollection, scope, consider, locateType, injectionContext);

                    if (strategyDelegate != null)
                    {
                        return strategyDelegate;
                    }
                }
            }

            var wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(locateType);

            if (wrapperCollection != null)
            {
                var primary = consider == null ?
                    wrapperCollection.GetPrimary()?.GetActivationStrategyDelegate(scope, this, locateType) : null;

                if (primary != null)
                {
                    return primary;
                }

                var strategyDelegate = GetStrategyFromCollection(strategyCollection, scope, consider, locateType, injectionContext);

                if (strategyDelegate != null)
                {
                    return strategyDelegate;
                }
            }

            if (isGeneric)
            {
                var generic = locateType.GetGenericTypeDefinition();

                wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(generic);

                if (wrapperCollection != null)
                {
                    var primary = consider == null ?
                        wrapperCollection.GetPrimary()?.GetActivationStrategyDelegate(scope, this, locateType) : null;

                    if (primary != null)
                    {
                        return primary;
                    }

                    var strategyDelegate = GetStrategyFromCollection(strategyCollection, scope, consider, locateType, injectionContext);

                    if (strategyDelegate != null)
                    {
                        return strategyDelegate;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find keyed delegate from strategies
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="locateType">locate type</param>
        /// <param name="consider">filter for strategies</param>
        /// <param name="key">key to use during locate</param>
        /// <returns>delegate</returns>
        protected virtual ActivationStrategyDelegate FindKeyedDelegate(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key)
        {
            var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(locateType);

            var strategy = collection?.GetKeyedStrategy(key);

            if (strategy != null)
            {
                return strategy.GetActivationStrategyDelegate(scope, this, locateType);
            }

            if (locateType.IsConstructedGenericType)
            {
                var openGeneric = locateType.GetGenericTypeDefinition();

                collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(openGeneric);

                strategy = collection?.GetKeyedStrategy(key);

                if (strategy != null)
                {
                    return strategy.GetActivationStrategyDelegate(scope, this, locateType);
                }
                else
                {
                    var wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(openGeneric);
                    if (wrapperCollection != null)
                    {
                        var wrapperStrategy = wrapperCollection.GetStrategies()
                            .OfType<IKeyWrapperActivationStrategy>()
                            .FirstOrDefault();

                        return wrapperStrategy?.GetActivationStrategyDelegate(scope, this, locateType, key);
                    }
                }
            }

            return null;
        }

        private void AddInjectionContextExpression(IActivationExpressionResult expressionContext)
        {
            var method = typeof(IInjectionContextCreator).GetRuntimeMethod(nameof(IInjectionContextCreator.CreateContext),
                    new[]
                    {
                        typeof(object)
                    });

            var newExpression = Expression.Call(Expression.Constant(_injectionContextCreator),
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

        private ActivationStrategyDelegate GetStrategyFromCollection<T>(IActivationStrategyCollection<T> strategyCollection, IInjectionScope scope, ActivationStrategyFilter consider, Type locateType, IInjectionContext injectionContext) where T : IWrapperOrExportActivationStrategy
        {
            foreach (var strategy in strategyCollection.GetStrategies())
            {
                if (strategy.HasConditions)
                {
                    var pass = true;

                    foreach (var condition in strategy.Conditions)
                    {
                        if (!condition.MeetsCondition(strategy, new StaticInjectionContext(locateType)))
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

                if (consider != null && !consider(strategy))
                {
                    continue;
                }

                var strategyDelegate = strategy.GetActivationStrategyDelegate(scope, this, locateType);

                if (strategyDelegate != null)
                {
                    return strategyDelegate;
                }
            }

            return null;
        }


        private ActivationStrategyDelegate LocateEnumerableStrategy(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key)
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
