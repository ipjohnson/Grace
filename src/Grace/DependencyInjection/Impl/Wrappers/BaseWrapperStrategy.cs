using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Base strategy for wrappers to inherit from
    /// </summary>
    public abstract class BaseWrapperStrategy : ConfigurableActivationStrategy, ICompiledWrapperStrategy
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate> ActivationDelegates 
            = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        private ImmutableHashTree<(Type, object), ActivationStrategyDelegate> KeyedActivationDelegates 
            = ImmutableHashTree<(Type, object), ActivationStrategyDelegate>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">scope for strategy</param>
        protected BaseWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        { }

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            return closedType != null
                ? ActivationConfiguration.CloneToType(closedType)
                : base.GetActivationConfiguration(activationType);
        }

        /// <summary>
        /// Get the type that is being wrapped
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>wrapped type</returns>
        public abstract Type GetWrappedType(Type type);

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.WrapperStrategy;

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <param name="key">activation key</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope,
            IActivationStrategyCompiler compiler,
            Type activationType,
            object key = null)
        {
            var returnValue = key == null
                ? ActivationDelegates.GetValueOrDefault(activationType)
                : KeyedActivationDelegates.GetValueOrDefault((activationType, key));

            if (returnValue != null)
            {
                return returnValue;
            }
            
            var request = compiler.CreateNewRequest(activationType, 1, scope);
            request.SetLocateKey(key);

            returnValue = CompileDelegate(compiler, request);

            if (returnValue == null)
            {
                return null;
            }

            
            if (key == null)
            {
                return ImmutableHashTree.ThreadSafeAdd(ref ActivationDelegates, activationType, returnValue);
            }
            else
            {
                return ImmutableHashTree.ThreadSafeAdd(ref KeyedActivationDelegates, (activationType, key), returnValue);
            }
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public abstract IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Compile a delegate
        /// </summary>
        /// <param name="compiler">compiler</param>
        /// <param name="request">activation request</param>
        protected virtual ActivationStrategyDelegate CompileDelegate(
            IActivationStrategyCompiler compiler,
            IActivationExpressionRequest request)
        {
            return GetActivationExpression(request.RequestingScope, request) is {} expression
                ? compiler.CompileDelegate(request.RequestingScope, expression)
                : null;
        }

        public static IKnownValueExpression CreateKnownValueExpression(
            IActivationExpressionRequest request, 
            Type argType, 
            string valueId, 
            string hintName = null, 
            int? position = null)
        {
            var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod(
                nameof(IExtraDataContainer.GetExtraData), 
                [typeof(object)]);

            var callExpression = Expression.Call(
                request.InjectionContextParameter, 
                getMethod,
                Expression.Constant(valueId));

            return new SimpleKnownValueExpression(
                argType, 
                Expression.Convert(callExpression, argType), 
                hintName, 
                position);
        }
    }
}
