using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class GenericCompiledExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly ILifestyleExpressionBuilder _builder;
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
        private ImmutableHashTree<Type, ICompiledLifestyle> _lifestyles = ImmutableHashTree<Type, ICompiledLifestyle>.Empty;

        public GenericCompiledExportStrategy(Type activationType, IInjectionScope injectionScope, ILifestyleExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }
        
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var configuration = ActivationConfiguration.CloneToType(closedType);

            configuration.Lifestyle = GetLifestyle(activationType);

            return configuration;
        }

        private ICompiledLifestyle GetLifestyle(Type activationType)
        {
            if (Lifestyle == null)
            {
                return null;
            }

            var lifestyle = _lifestyles.GetValueOrDefault(activationType);

            if (lifestyle != null)
            {
                return lifestyle;
            }

            lifestyle = Lifestyle.Clone();

            return ImmutableHashTree.ThreadSafeAdd(ref _lifestyles, activationType, lifestyle);
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

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            throw new NotImplementedException();
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var activationType = request.ActivationType;

            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var activation = GetActivationConfiguration(closedType);

            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this);

            return result ?? _builder.GetActivationExpression(scope, request, activation, activation.Lifestyle);
        }

        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }
    }
}
