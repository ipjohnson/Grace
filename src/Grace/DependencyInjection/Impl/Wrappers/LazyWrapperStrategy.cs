using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Wrapper strategy for Lazy&lt;T&gt;
    /// </summary>
    public class LazyWrapperStrategy : BaseWrapperStrategy
    {
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public LazyWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Lazy<>), injectionScope)
        {

        }

        /// <summary>
        /// Get type that wrapper wraps
        /// </summary>
        /// <param name="type">wrapper type</param>
        /// <returns>type that has been wrapped</returns>
        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Lazy<>) ? type.GetTypeInfo().GenericTypeArguments[0] : null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(LazyExpression<>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod(nameof(LazyExpression<object>.CreateLazy), new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, this);

            request.RequireExportScope();
            request.RequireDisposalScope();

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.ScopeParameter,
                    request.DisposalScopeExpression, request.InjectionContextParameter);

            request.RequireInjectionContext();

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        /// <summary>
        /// Lazy expression helper class
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public class LazyExpression<TResult>
        {
            private ActivationStrategyDelegate _delegate;
            private IActivationExpressionRequest _request;
            private IInjectionScope _scope;
            private IActivationStrategy _activationStrategy;
            private readonly object _lock = new object();

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <param name="activationStrategy"></param>
            public LazyExpression(IInjectionScope scope, IActivationExpressionRequest request, IActivationStrategy activationStrategy)
            {
                _scope = scope;
                _request = request;
                _activationStrategy = activationStrategy;
            }

            /// <summary>
            /// Create lazy instance
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="disposalScope"></param>
            /// <param name="injectionContext"></param>
            /// <returns></returns>
            public Lazy<TResult> CreateLazy(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext injectionContext)
            {
                return new Lazy<TResult>(() =>
                {
                    if (_delegate == null)
                    {
                        _delegate = CompileDelegate();
                    }

                    return (TResult) _delegate(scope, disposalScope, injectionContext);
                });
            }

            private ActivationStrategyDelegate CompileDelegate()
            {
                lock (_lock)
                {
                    if (_delegate == null)
                    {
                        var requestType = _request.ActivationType.GenericTypeArguments[0];

                        var newRequest = _request.NewRequest(requestType, _activationStrategy, typeof(Lazy<TResult>),
                            RequestType.Other, null, true);

                        newRequest.DisposalScopeExpression = _request.Constants.RootDisposalScope;

                        var activationExpression = _request.Services.ExpressionBuilder.GetActivationExpression(_scope,
                            newRequest);

                        _delegate = _request.Services.Compiler.CompileDelegate(_scope, activationExpression);

                        _scope = null;
                        _request = null;
                        _activationStrategy = null;
                    }
                }

                return _delegate;
            }
        }
    }
}
