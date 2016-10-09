using System;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class CompiledDecoratorStrategy : ConfigurableActivationStrategy, ICompiledDecoratorStrategy
    {
        private readonly ILifestyleExpressionBuilder _builder;
        

        public CompiledDecoratorStrategy(Type activationType, IInjectionScope injectionScope, ILifestyleExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }
        
        public bool ApplyAfterLifestyle { get; set; }
        

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return _builder.GetActivationExpression(scope, request, ActivationConfiguration, lifestyle);
        }
    }
}
