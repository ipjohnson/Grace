using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Data;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class FuncTwoArgWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        public FuncTwoArgWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Func<,,>), injectionScope)
        {
        }

        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Func<,,>) ? type.GetTypeInfo().GenericTypeArguments[2] : null;
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(FuncExpression<,,>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod("CreateFunc", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }


        public class FuncExpression<T1, T2, TResult>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _t1Id = Guid.NewGuid().ToString();
            private readonly string _t2Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;

            public FuncExpression(IInjectionScope scope, IActivationExpressionRequest request,
                IInjectionContextCreator injectionContextCreator)
            {
                _injectionContextCreator = injectionContextCreator;

                var arg1Type = request.ActivationType.GenericTypeArguments[0];
                var arg2Type = request.ActivationType.GenericTypeArguments[1];
                var requestType = request.ActivationType.GenericTypeArguments[2];

                var newRequest = request.NewRequest(requestType, request.InjectedType, RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, arg1Type, _t1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, arg2Type, _t2Id));

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);
            }

            private IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request, Type argType, string valueId)
            {
                var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod("GetExtraData", new[] { typeof(object) });

                var callExpression = Expression.Call(request.Constants.InjectionContextParameter, getMethod,
                    Expression.Constant(valueId));

                return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType));
            }

            public Func<T1, T2, TResult> CreateFunc(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                return (arg1, arg2) =>
                {
                    var newContext = context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_t1Id, arg1);
                    newContext.SetExtraData(_t2Id, arg2);

                    return (TResult)_action(scope, disposalScope, newContext);
                };
            }
        }
    }
}
