using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Base strategy for wrappers that wrap keyed service to inherit from
    /// </summary>
    public abstract class BaseKeyWrapperStrategy : ConfigurableActivationStrategy, IKeyWrapperActivationStrategy
    {
        /// <summary>
        /// Activation delegates, by type and locate key.
        /// </summary>
        protected ImmutableHashTree<Tuple<Type, object>, ActivationStrategyDelegate> ActivationDelegates = 
            ImmutableHashTree<Tuple<Type, object>, ActivationStrategyDelegate>.Empty;


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">scope for strategy</param>
        protected BaseKeyWrapperStrategy(Type activationType, IInjectionScope injectionScope) 
            : base(activationType, injectionScope)
        {
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType => ActivationStrategyType.WrapperStrategy;

        /// <summary>
        /// Get the type that is being wrapped
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>wrapped type</returns>
        public abstract Type GetWrappedType(Type type);

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract IActivationExpressionResult GetActivationExpression(
            IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            if (closedType != null)
            {
                return ActivationConfiguration.CloneToType(closedType);
            }

            return base.GetActivationConfiguration(activationType);
        }

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <param name="key">The locate key.</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType, object key)
        {
            var pair = new Tuple<Type, object>(activationType, key);
            var returnValue = ActivationDelegates.GetValueOrDefault(pair);

            if (returnValue != null)
            {
                return returnValue;
            }

            returnValue = CompileDelegate(scope, compiler, pair);

            if (returnValue != null)
            {
                returnValue = ImmutableHashTree.ThreadSafeAdd(ref ActivationDelegates, pair, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
            => GetActivationStrategyDelegate(scope, compiler, activationType, null);

        /// <summary>
        /// Compile a delegate
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="compiler">compiler</param>
        /// <param name="pair">Pair of activation type and locate key.</param>
        /// <returns></returns>
        protected virtual ActivationStrategyDelegate CompileDelegate(
            IInjectionScope scope, 
            IActivationStrategyCompiler compiler,
            Tuple<Type, object> pair)
        {
            var request = compiler.CreateNewRequest(pair.Item1, 1, scope);

            if (pair.Item2 != null)
                request.SetLocateKey(pair.Item2);

            var expressionResult = GetActivationExpression(scope, request);

            ActivationStrategyDelegate returnValue = null;

            if (expressionResult != null)
            {
                returnValue = compiler.CompileDelegate(scope, expressionResult);
            }

            return returnValue;
        }
    }
}
