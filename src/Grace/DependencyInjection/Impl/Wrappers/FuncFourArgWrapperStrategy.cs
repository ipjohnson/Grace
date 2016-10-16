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
    public class FuncFourArgWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        public FuncFourArgWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Func<,,,,>), injectionScope)
        {
        }

        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Func<,,,>) ? type.GetTypeInfo().GenericTypeArguments[4] : null;
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(FuncExpression<,,,,>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod("CreateFunc", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator, this);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        public class FuncExpression<T1, T2, T3, T4, TResult>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _t1Id = Guid.NewGuid().ToString();
            private readonly string _t2Id = Guid.NewGuid().ToString();
            private readonly string _t3Id = Guid.NewGuid().ToString();
            private readonly string _t4Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;

            public FuncExpression(IInjectionScope scope, IActivationExpressionRequest request,
                IInjectionContextCreator injectionContextCreator, IActivationStrategy activationStrategy)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), activationStrategy, typeof(Func<T1,T2,T3,T4,TResult>), RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T1), _t1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T2), _t2Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T3), _t3Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T4), _t4Id));

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

            public Func<T1, T2, T3, T4, TResult> CreateFunc(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                return (arg1, arg2, arg3, arg4) =>
                {
                    var newContext = context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_t1Id, arg1);
                    newContext.SetExtraData(_t2Id, arg2);
                    newContext.SetExtraData(_t3Id, arg3);
                    newContext.SetExtraData(_t4Id, arg4);

                    return (TResult)_action(scope, disposalScope, newContext);
                };
            }
        }
    }
}
