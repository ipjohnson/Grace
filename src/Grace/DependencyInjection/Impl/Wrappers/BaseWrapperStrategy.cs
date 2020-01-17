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
        /// <summary>
        /// Activation delegates
        /// </summary>
        protected ImmutableHashTree<Type, ActivationStrategyDelegate> ActivationDelegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">scope for strategy</param>
        protected BaseWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {

        }

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
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope,
                                                                        IActivationStrategyCompiler compiler,
                                                                        Type activationType)
        {
            var returnValue = ActivationDelegates.GetValueOrDefault(activationType);

            if (returnValue != null)
            {
                return returnValue;
            }

            returnValue = CompileDelegate(scope, compiler, activationType);

            if (returnValue != null)
            {
                returnValue = ImmutableHashTree.ThreadSafeAdd(ref ActivationDelegates, activationType, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Compile a delegate
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="compiler">compiler</param>
        /// <param name="activationType">activation type</param>
        /// <returns></returns>
        protected virtual ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var request = compiler.CreateNewRequest(activationType, 1, scope);

            var expressionResult = GetActivationExpression(scope, request);

            ActivationStrategyDelegate returnValue = null;

            if (expressionResult != null)
            {
                returnValue = compiler.CompileDelegate(scope, expressionResult);
            }

            return returnValue;
        }


        public static IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request, Type argType, string valueId, string hintName = null, int? position = null)
        {
            var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod(nameof(IExtraDataContainer.GetExtraData), new[] { typeof(object) });

            var callExpression = Expression.Call(request.InjectionContextParameter, getMethod,
                Expression.Constant(valueId));

            return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType), hintName, position);
        }
    }
}
