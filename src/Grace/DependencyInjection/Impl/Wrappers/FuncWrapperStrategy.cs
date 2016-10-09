using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class FuncWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        public FuncWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Func<>), injectionScope)
        {
        }
        
        public override Type GetWrappedType(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                return type.GenericTypeArguments[0];
            }

            return null;
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(FuncExpression<>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod("CreateFunc", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }


        public class FuncExpression<TResult>
        {
            private readonly ActivationStrategyDelegate _action;

            public FuncExpression(IInjectionScope scope, IActivationExpressionRequest request)
            {
                var requestType = request.ActivationType.GenericTypeArguments[0];

                var newRequest = request.NewRequest(requestType, request.InjectedType, RequestType.Other, null, true);

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);
            }

            public Func<TResult> CreateFunc(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context)
            {
                return () => (TResult)_action(scope, disposalScope, context);
            }
        }
    }
}
