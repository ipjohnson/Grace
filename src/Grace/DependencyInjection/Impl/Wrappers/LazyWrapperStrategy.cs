using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class LazyWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        public LazyWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Lazy<>), injectionScope)
        {
        }

        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Lazy<>) ? type.GetTypeInfo().GenericTypeArguments[0] : null;
        }

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

        public class LazyExpression<TResult>
        {
            private readonly ActivationStrategyDelegate _delegate;

            public LazyExpression(IInjectionScope scope, IActivationExpressionRequest request, IActivationStrategy activationStrategy)
            {
                var requestType = request.ActivationType.GenericTypeArguments[0];

                var newRequest = request.NewRequest(requestType, activationStrategy, typeof(Lazy<TResult>), RequestType.Other, null, true);

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _delegate = request.Services.Compiler.CompileDelegate(scope, activationExpression);
            }

            public Lazy<TResult> CreateLazy(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext injectionContext)
            {
                return new Lazy<TResult>(() => (TResult)_delegate(scope,disposalScope,injectionContext));
            }
        }
    }
}
