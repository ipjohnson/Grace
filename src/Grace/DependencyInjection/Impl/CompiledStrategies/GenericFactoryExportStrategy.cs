using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.CompiledStrategies
{
    public class GenericFactoryExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private readonly Func<Type, object> _factory;
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;
        private ImmutableHashTree<Type, ICompiledLifestyle> _lifestyles = ImmutableHashTree<Type, ICompiledLifestyle>.Empty;
        private ImmutableLinkedList<ICompiledExportStrategy> _secondaryStrategies = ImmutableLinkedList<ICompiledExportStrategy>.Empty;

        public GenericFactoryExportStrategy(IInjectionScope scope, Type activationType, Func<Type, object> factory) : base(activationType, scope)
        {
            _factory = factory;
        }

        /// <summary>
        /// Dispose of strategy
        /// </summary>
        public override void Dispose()
        {
            var lifestyles = Interlocked.Exchange(ref _lifestyles, ImmutableHashTree<Type, ICompiledLifestyle>.Empty);

            foreach (var valuePair in lifestyles)
            {
                var disposable = valuePair.Value as IDisposable;

                disposable?.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Get activation configuration for strategy
        /// </summary>
        /// <param name="activationType"></param>
        /// <returns></returns>
        public override TypeActivationConfiguration GetActivationConfiguration(Type activationType)
        {
            var closedType = ReflectionHelper.CreateClosedExportTypeFromRequestingType(ActivationType, activationType);

            var configuration = ActivationConfiguration.CloneToType(closedType);

            configuration.Lifestyle = GetLifestyle(activationType);

            return configuration;
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
            var objectDelegate = _delegates.GetValueOrDefault(activationType);

            if (objectDelegate != null)
            {
                return objectDelegate;
            }

            var request = compiler.CreateNewRequest(activationType, 1, scope);

            var expression = GetActivationExpression(scope, request);

            if (expression == null)
            {
                return null;
            }

            objectDelegate = compiler.CompileDelegate(scope, expression);

            ImmutableHashTree.ThreadSafeAdd(ref _delegates, activationType, objectDelegate);

            return objectDelegate;
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
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var activationType = request.ActivationType;

            var activation = GetActivationConfiguration(activationType);

            if (activation.Lifestyle == null)
            {
                return CreateExpression(request, activationType);
            }

            return activation.Lifestyle.ProvideLifestyleExpression(scope, request, r => CreateExpression(r, activationType));
        }

        private IActivationExpressionResult CreateExpression(IActivationExpressionRequest request, Type activationType)
        {
            Expression expression;

            if (_factory.Target != null)
            {
                expression = Expression.Call(Expression.Constant(_factory.Target), _factory.GetMethodInfo(),
                    Expression.Constant(activationType));
            }
            else
            {
                expression = Expression.Call(_factory.GetMethodInfo(), Expression.Constant(activationType));
            }

            if (expression.Type != activationType)
            {
                expression = Expression.Convert(expression, activationType);
            }

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

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
    }
}