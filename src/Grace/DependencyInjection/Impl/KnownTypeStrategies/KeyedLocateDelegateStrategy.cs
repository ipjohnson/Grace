using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.KnownTypeStrategies
{
    /// <summary>
    /// Strategy for creating a KeyedLocateDelegate delegate
    /// </summary>
    public class KeyedLocateDelegateStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public KeyedLocateDelegateStrategy(IInjectionScope injectionScope) : base(typeof(KeyedLocateDelegate<,>), injectionScope)
        {
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.FrameworkExportStrategy;

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            throw new NotSupportedException("Decorators not currently supported on KeyedLocateDelegate<,>");
        }

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var objectDelegate = _delegates.GetValueOrDefault(activationType);

            if (objectDelegate != null)
            {
                return objectDelegate;
            }

            var request = compiler.CreateNewRequest(activationType, 1, scope);

            var expression = GetActivationExpression(scope, request);

            objectDelegate = compiler.CompileDelegate(scope, expression);

            ImmutableHashTree.ThreadSafeAdd(ref _delegates, activationType, objectDelegate);

            return objectDelegate;
        }


        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var openMethod = typeof(KeyedLocateDelegateStrategy).GetRuntimeMethod(nameof(CreateKeyedDelegate),
                new[] { typeof(IExportLocatorScope) });

            var closedMethod = openMethod.MakeGenericMethod(request.ActivationType.GenericTypeArguments);

            request.RequireExportScope();

            var expression = Expression.Call(null, closedMethod, request.ScopeParameter);

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            throw new NotSupportedException("This type of export does not support secondary strategies");
        }

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }

        /// <summary>
        /// Creates a new keyed delegate
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="locatorScope"></param>
        /// <returns></returns>
        public static KeyedLocateDelegate<TKey, TValue> CreateKeyedDelegate<TKey, TValue>(
            IExportLocatorScope locatorScope)
        {
            return key => locatorScope.Locate<TValue>(withKey: key);
        }
    }
}
