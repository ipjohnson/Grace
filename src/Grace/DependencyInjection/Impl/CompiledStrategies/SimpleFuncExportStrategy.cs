using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// This strategy doesn't honor lifestyle or decorators, it's intended to be very simple
    /// </summary>
    public class SimpleFuncExportStrategy<T> : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly Func<IExportLocatorScope, T> _func;
        private readonly ActivationStrategyDelegate _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="func"></param>
        /// <param name="injectionScope">owning injection scope</param>
        public SimpleFuncExportStrategy(Func<IExportLocatorScope, T> func, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _func = func;
            _delegate = (scope, disposalScope, context) => func(injectionScope);
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            throw new NotSupportedException("THis strategy type does not support decorators");
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
            return _delegate;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            Expression expression;

            request.RequireExportScope();

            if (_func.Target != null)
            {
                expression = Expression.Call(Expression.Constant(_func.Target), _func.GetMethodInfo(),
                    request.ScopeParameter);
            }
            else
            {
                expression = Expression.Call(_func.GetMethodInfo(), request.ScopeParameter);
            }

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
            throw new NotSupportedException("Secondary strategies are not supported on this type");
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
