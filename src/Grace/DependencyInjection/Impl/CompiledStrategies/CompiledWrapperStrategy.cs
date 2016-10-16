using System;
using System.Reflection;
using System.Threading;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class CompiledWrapperStrategy : ConfigurableActivationStrategy, IConfigurableCompiledWrapperStrategy
    {
        private readonly ILifestyleExpressionBuilder _builder;
        private ActivationStrategyDelegate _delegate;
        private Type _wrappedType;
        private int _genericArgPosition = -1;

        public CompiledWrapperStrategy(Type activationType, IInjectionScope injectionScope, ILifestyleExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.WrapperStrategy;

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            if (_delegate != null)
            {
                return _delegate;
            }

            var request = GetActivationExpression(scope, compiler.CreateNewRequest(activationType, 1));

            var compiledDelegate = compiler.CompileDelegate(scope, request);

            Interlocked.CompareExchange(ref _delegate, compiledDelegate, null);

            return _delegate;
        }

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return _builder.GetActivationExpression(scope, request, ActivationConfiguration, lifestyle);
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return _builder.GetActivationExpression(scope, request, ActivationConfiguration, ActivationConfiguration.Lifestyle);
        }

        public Type GetWrappedType(Type type)
        {
            if (type.IsConstructedGenericType && 
                _genericArgPosition >= 0 && 
                ActivationType.GetTypeInfo().IsGenericTypeDefinition)
            {
                return type.GenericTypeArguments[_genericArgPosition];
            }

            return _wrappedType;
        }

        public void SetWrappedType(Type type)
        {
            _wrappedType = type;
        }

        public void SetWrappedGenericArgPosition(int argPosition)
        {
            _genericArgPosition = argPosition;
        }
    }
}
