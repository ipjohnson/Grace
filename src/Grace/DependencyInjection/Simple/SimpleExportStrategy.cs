using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Simple
{
    public class SimpleExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        public SimpleExportStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            throw new NotImplementedException();
        }

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            throw new NotImplementedException();
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            throw new NotSupportedException("Adding secondary strategies is not supported for simple strategies");
        }

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }
    }
}
