using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.EnumerableStrategies
{
    public abstract class BaseGenericEnumerableStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        protected BaseGenericEnumerableStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        public abstract IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle);

        public virtual ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
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

        public abstract IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            throw new NotImplementedException();
        }
    }
}
