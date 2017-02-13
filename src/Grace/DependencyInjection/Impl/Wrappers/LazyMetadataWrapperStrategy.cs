using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Wrapper for Lazy&lt;T,IActivationStrategyMetadata&gt;
    /// </summary>
    public class LazyMetadataWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public LazyMetadataWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Lazy<,>), injectionScope)
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

            return genericType == typeof(Lazy<,>) && type.GetTypeInfo().GenericTypeArguments[1] == typeof(IActivationStrategyMetadata) ?
                    type.GetTypeInfo().GenericTypeArguments[0] : null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(LazyExpression<>).MakeGenericType(request.ActivationType.GenericTypeArguments[0]);

            var closedMethod = closedClass.GetRuntimeMethod("CreateLazy", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var wrappedStrategy = request.GetWrappedStrategy();

            if (wrappedStrategy == null)
            {
                throw new LocateException(request.GetStaticInjectionContext(), "Could not find strategy that is wrapped");
            }

            var instance = Activator.CreateInstance(closedClass, scope, request, this, wrappedStrategy.Metadata);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

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
            private readonly IActivationStrategyMetadata _metadata;
            private readonly object _lock = new object();

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <param name="activationStrategy"></param>
            /// <param name="metadata"></param>
            public LazyExpression(IInjectionScope scope, IActivationExpressionRequest request, IActivationStrategy activationStrategy, IActivationStrategyMetadata metadata)
            {
                _scope = scope;
                _request = request;
                _activationStrategy = activationStrategy;
                _metadata = metadata;
            }

            /// <summary>
            /// Create lazy instance
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="disposalScope"></param>
            /// <param name="injectionContext"></param>
            /// <returns></returns>
            public Lazy<TResult, IActivationStrategyMetadata> CreateLazy(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext injectionContext)
            {
                return new Lazy<TResult, IActivationStrategyMetadata>(() =>
                {
                    if (_delegate == null)
                    {
                        _delegate = CompileDelegate();
                    }

                    return (TResult)_delegate(scope, disposalScope, injectionContext);
                }, _metadata);
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
