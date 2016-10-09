using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    public abstract class BaseInstanceExportStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        protected ActivationStrategyDelegate StrategyDelegate;

        protected BaseInstanceExportStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var result = request.Services.ExpressionBuilder.DecorateExportStrategy(scope, request, this);

            return result ?? CreateExpression(scope, request, Lifestyle);
        }

        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType)
        {
            if (StrategyDelegate != null)
            {
                return StrategyDelegate;
            }

            var  request = compiler.CreateNewRequest(activationType, 1);

            var expression = GetActivationExpression(scope, request);

            var newDelegate = compiler.CompileDelegate(scope, expression);

            return Interlocked.CompareExchange(ref StrategyDelegate, newDelegate, null);
        }

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope,
            IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            return CreateExpression(scope, request, lifestyle);
        }

        protected abstract IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle);

        public Expression ApplyNullCheckAndAddDisposal(IInjectionScope scope, IActivationExpressionRequest request, Expression expression)
        {
            if (ExternallyOwned)
            {
                if (!scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull())
                {
                    var closedMethod = CheckForNullMethodInfo.MakeGenericMethod(request.ActivationType);
                    
                    return Expression.Call(closedMethod, expression);
                }
            }
            else
            {
                if (scope.ScopeConfiguration.Behaviors.AllowInstanceAndFactoryToReturnNull())
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
