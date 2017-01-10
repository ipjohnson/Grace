using System;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Wrapper strategy for generic type
    /// </summary>
    public class GenericCompiledWrapperStrategy : ConfigurableActivationStrategy, IConfigurableCompiledWrapperStrategy
    {
        private readonly IDefaultStrategyExpressionBuilder _builder;
        private int _wrappedGenericArgPosition;
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="builder"></param>
        public GenericCompiledWrapperStrategy(Type activationType, IInjectionScope injectionScope, IDefaultStrategyExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.WrapperStrategy;

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            return ActivationConfiguration.CloneToType(closedType);
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
            var activationType = request.ActivationType;

            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var activation = GetActivationConfiguration(closedType);
            
            return _builder.GetActivationExpression(scope, request, activation, activation.Lifestyle);
        }

        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="type">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        public Type GetWrappedType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var args = type.GenericTypeArguments;

                if (args.Length > _wrappedGenericArgPosition)
                {
                    return args[_wrappedGenericArgPosition];
                }
            }

            return null;
        }


        /// <summary>
        /// Set the type that is being wrapped
        /// </summary>
        /// <param name="type"></param>
        public void SetWrappedType(Type type)
        {

        }

        /// <summary>
        /// Set the position of the generic arg that is being wrapped
        /// </summary>
        /// <param name="argPosition"></param>
        public void SetWrappedGenericArgPosition(int argPosition)
        {
            if (argPosition < 0) throw new ArgumentOutOfRangeException(nameof(argPosition),"arg position must be >= 0");

            _wrappedGenericArgPosition = argPosition;
        }
    }
}
