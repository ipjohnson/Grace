using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class FuncOneArgWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        

        public FuncOneArgWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Func<,>), injectionScope)
        {
        
        }

        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Func<,>) ? type.GetTypeInfo().GenericTypeArguments[1] : null;
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(FuncExpression<,>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod("CreateFunc", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator, this);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        public class FuncExpression<TArg1, TResult>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _arg1Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;

            public FuncExpression(IInjectionScope scope, IActivationExpressionRequest request,
                IInjectionContextCreator injectionContextCreator, IActivationStrategy activationStrategy)
            {
                _injectionContextCreator = injectionContextCreator;
                var requestType = request.ActivationType.GenericTypeArguments[1];

                var newRequest = request.NewRequest(requestType, activationStrategy, typeof(Func<TArg1,TResult>), RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request));

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);
            }

            private IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request)
            {
                var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod("GetExtraData", new[] { typeof(object) });

                var argType = request.ActivationType.GenericTypeArguments[0];

                var callExpression = Expression.Call(request.Constants.InjectionContextParameter, getMethod, Expression.Constant(_arg1Id));

                return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType));
            }

            public Func<TArg1, TResult> CreateFunc(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context)
            {
                return arg1 =>
                {
                    var newContext = context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_arg1Id, arg1);

                    return (TResult)_action(scope, disposalScope, newContext);
                };
            }

        }
    }
}
