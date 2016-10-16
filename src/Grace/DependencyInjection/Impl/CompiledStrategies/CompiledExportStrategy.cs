using System;
using System.Collections.Generic;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class CompiledExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly ILifestyleExpressionBuilder _builder;
        private ActivationStrategyDelegate _delegate;

        public CompiledExportStrategy(Type activationType, IInjectionScope injectionScope, ILifestyleExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
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
            IActivationExpressionResult result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this);

            return result ?? _builder.GetActivationExpression(scope, request, ActivationConfiguration, ActivationConfiguration.Lifestyle);
        }

        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }
    }
}
