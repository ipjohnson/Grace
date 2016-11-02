using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Base export strategy for all instance and factory exports
    /// </summary>
    public abstract class BaseInstanceExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private ImmutableLinkedList<ICompiledExportStrategy> _secondaryStrategies = ImmutableLinkedList<ICompiledExportStrategy>.Empty;

        /// <summary>
        /// delegate for the strategy
        /// </summary>
        protected ActivationStrategyDelegate StrategyDelegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        protected BaseInstanceExportStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this);

            return result ?? CreateExpression(scope, request, Lifestyle);
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

        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
        {
            if (StrategyDelegate != null)
            {
                return StrategyDelegate;
            }

            var request = compiler.CreateNewRequest(activationType, 1);

            var expression = GetActivationExpression(scope, request);

            var newDelegate = compiler.CompileDelegate(scope, expression);

            return Interlocked.CompareExchange(ref StrategyDelegate, newDelegate, null);
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return CreateExpression(scope, request, lifestyle);
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected abstract IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle);

        /// <summary>
        /// Applies null check and disposal scope tracking logic to an expression
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected Expression ApplyNullCheckAndAddDisposal(IInjectionScope scope, IActivationExpressionRequest request, Expression expression)
        {
            if (expression.Type != request.ActivationType &&
               !ReflectionService.CheckTypeIsBasedOnAnotherType(expression.Type, request.ActivationType))
            {
                expression = Expression.Convert(expression, request.ActivationType);
            }

            if (ExternallyOwned)
            {
                if (!scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull)
                {
                    var closedMethod = CheckForNullMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod,
                                           Expression.Constant(request.GetStaticInjectionContext()),
                                           expression);
                }
            }
            else
            {
                if (scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull)
                {
                    var closedMethod = AddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod, request.DisposalScopeExpression, expression);
                }
                else
                {
                    var closedMethod =
                        CheckForNullAndAddToDisposalScopeMethodInfo.MakeGenericMethod(request.ActivationType);

                    return Expression.Call(closedMethod,
                                           request.DisposalScopeExpression,
                                           Expression.Constant(request.GetStaticInjectionContext()), expression);
                }
            }

            return expression;
        }

        #region Check For Null
        /// <summary>
        /// Check value for null
        /// </summary>
        /// <typeparam name="T">type of value</typeparam>
        /// <param name="context">static context</param>
        /// <param name="value">value to check</param>
        /// <returns>non null value</returns>
        public static T CheckForNull<T>(StaticInjectionContext context, T value)
        {
            if (value == null)
            {
                throw new NullValueProvidedException(context);
            }

            return value;
        }

        private static MethodInfo _checkForNullMethodInfo;

        public static MethodInfo CheckForNullMethodInfo
        {
            get
            {
                return _checkForNullMethodInfo ??
                       (_checkForNullMethodInfo =
                           typeof(BaseInstanceExportStrategy).GetRuntimeMethods().First(m => m.Name == "CheckForNull"));
            }
        }

        #endregion

        #region AddToDisposalScope
        public static T AddToDisposalScope<T>(IDisposalScope disposalScope, T value)
        {
            var disposable = value as IDisposable;

            if (disposable != null)
            {
                disposalScope.AddDisposable(disposable);
            }

            return value;
        }

        private static MethodInfo _addToDisposalScopeMethodInfo;

        public static MethodInfo AddToDisposalScopeMethodInfo
        {
            get
            {
                return _addToDisposalScopeMethodInfo ??
                       (_addToDisposalScopeMethodInfo =
                           typeof(BaseInstanceExportStrategy).GetRuntimeMethods().First(m => m.Name == "AddToDisposalScope"));
            }
        }
        #endregion

        #region CheckForNullAndAddToDisposalScope
        public static T CheckForNullAndAddToDisposalScope<T>(IDisposalScope disposalScope,
            StaticInjectionContext context, T value)
        {
            if (value == null)
            {
                throw new NullValueProvidedException(context);
            }

            var disposable = value as IDisposable;

            if (disposable != null)
            {
                disposalScope.AddDisposable(disposable);
            }

            return value;
        }

        private static MethodInfo _checkForNullAndAddToDisposalScopeMethodInfo;

        public static MethodInfo CheckForNullAndAddToDisposalScopeMethodInfo
        {
            get
            {
                return _checkForNullAndAddToDisposalScopeMethodInfo ??
                       (_checkForNullAndAddToDisposalScopeMethodInfo =
                           typeof(BaseInstanceExportStrategy).GetRuntimeMethods().First(m => m.Name == "CheckForNullAndAddToDisposalScope"));
            }
        }
        #endregion

    }
}
