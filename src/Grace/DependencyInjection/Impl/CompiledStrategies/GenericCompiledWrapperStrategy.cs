using System;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class GenericCompiledWrapperStrategy : ConfigurableActivationStrategy, IConfigurableCompiledWrapperStrategy
    {
        private readonly ILifestyleExpressionBuilder _builder;
        private int _wrappedGenericArgPosition;
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        public GenericCompiledWrapperStrategy(Type activationType, IInjectionScope injectionScope, ILifestyleExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.WrapperStrategy;

        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            return ActivationConfiguration.CloneToType(closedType);
        }

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var objectDelegate = _delegates.GetValueOrDefault(activationType);

            if (objectDelegate != null)
            {
                return objectDelegate;
            }

            var request = compiler.CreateNewRequest(activationType, 1);

            var expression = GetActivationExpression(scope, request);

            objectDelegate = compiler.CompileDelegate(scope, expression);

            ImmutableHashTree.ThreadSafeAdd(ref _delegates, activationType, objectDelegate);

            return objectDelegate;
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var activationType = request.ActivationType;

            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var activation = GetActivationConfiguration(closedType);
            
            return _builder.GetActivationExpression(scope, request, activation, activation.Lifestyle);
        }

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


        public void SetWrappedType(Type type)
        {

        }

        public void SetWrappedGenericArgPosition(int argPosition)
        {
            if (argPosition < 0) throw new ArgumentOutOfRangeException(nameof(argPosition),"arg position must be >= 0");

            _wrappedGenericArgPosition = argPosition;
        }
    }
}
