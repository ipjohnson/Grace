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

            var closedMethod = closedClass.GetRuntimeMethod("CreateLazy", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, this);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        /// <summary>
        /// Lazy expression helper class
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        public class LazyExpression<TResult>
        {
            private readonly ActivationStrategyDelegate _delegate;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <param name="activationStrategy"></param>
            public LazyExpression(IInjectionScope scope, IActivationExpressionRequest request, IActivationStrategy activationStrategy)
            {
                var requestType = request.ActivationType.GenericTypeArguments[0];

                var newRequest = request.NewRequest(requestType, activationStrategy, typeof(Lazy<TResult>), RequestType.Other, null, true);

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _delegate = request.Services.Compiler.CompileDelegate(scope, activationExpression);
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
                return new Lazy<TResult>(() => (TResult)_delegate(scope,disposalScope,injectionContext));
            }
        }
    }
}
