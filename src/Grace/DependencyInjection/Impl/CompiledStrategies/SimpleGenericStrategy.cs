using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Abstract class for basing simple generic strategies on
    /// </summary>
    public abstract class SimpleGenericStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
        private ImmutableLinkedList<ICompiledExportStrategy> _secondaryStrategies = ImmutableLinkedList<ICompiledExportStrategy>.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        protected SimpleGenericStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {

        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public abstract IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle);

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public virtual ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
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
        public abstract IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            if (secondaryStrategy == null) throw new ArgumentNullException(nameof(secondaryStrategy));

            _secondaryStrategies = _secondaryStrategies.Add(secondaryStrategy);
        }

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return _secondaryStrategies;
        }
    }
}
