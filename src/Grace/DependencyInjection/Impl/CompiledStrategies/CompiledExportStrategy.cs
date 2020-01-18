using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    /// <summary>
    /// Standard export strategy for all non generic types registered with the container
    /// </summary>
    public class CompiledExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly IDefaultStrategyExpressionBuilder _builder;
        private ImmutableLinkedList<ICompiledExportStrategy> _secondaryStrategies = ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        private ActivationStrategyDelegate _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        /// <param name="builder"></param>
        public CompiledExportStrategy(Type activationType, IInjectionScope injectionScope, IDefaultStrategyExpressionBuilder builder) : base(activationType, injectionScope)
        {
            _builder = builder;
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Dependencies needed to activate strategy
        /// </summary>
        public override IEnumerable<ActivationStrategyDependency> GetDependencies(IActivationExpressionRequest request)
        {
            if (request == null)
            {
                request = InjectionScope.StrategyCompiler.CreateNewRequest(ActivationType, 0, InjectionScope);
            }

            return _builder.TypeExpressionBuilder.GetDependencies(ActivationConfiguration, request);
        }

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
        {
            if (_delegate != null)
            {
                return _delegate;
            }

            scope.ScopeConfiguration.Trace?.Invoke($"Activating {ActivationType.FullName} with lifestyle '{Lifestyle}' for request type {activationType.FullName}");

            var request = GetActivationExpression(scope, compiler.CreateNewRequest(activationType, 1, scope));

            var compiledDelegate = compiler.CompileDelegate(scope, request);

            Interlocked.CompareExchange(ref _delegate, compiledDelegate, null);

            return _delegate;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return _builder.GetActivationExpression(scope, request, ActivationConfiguration, lifestyle);
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            Expression assignStatement = null;
            ParameterExpression newScope = null;

            if (ActivationConfiguration.CustomScopeName != null)
            {
                request.RequireExportScope();

                newScope = Expression.Variable(typeof(IExportLocatorScope));

                var beginScopeMethod =
                    typeof(IExportLocatorScope).GetTypeInfo().GetDeclaredMethod(nameof(IExportLocatorScope.BeginLifetimeScope));

                assignStatement =
                    Expression.Assign(newScope,
                        Expression.Call(request.ScopeParameter, beginScopeMethod,
                            Expression.Constant(ActivationConfiguration.CustomScopeName)));

                request.ScopeParameter = newScope;
            }

            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this) ??
                         _builder.GetActivationExpression(scope, request, ActivationConfiguration, ActivationConfiguration.Lifestyle);

            if (newScope != null)
            {
                result.AddExtraParameter(newScope);
                result.AddExtraExpression(assignStatement, true);
            }

            return result;
        }

        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        public void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy)
        {
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
