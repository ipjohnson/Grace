using Grace.Data;
using Grace.DependencyInjection.Impl.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class DelegateFiveArgWrapperStrategy : BaseWrapperStrategy
    {
        public DelegateFiveArgWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        public override Type GetWrappedType(Type type)
        {
            var invokeMethod = type.GetTypeInfo().GetDeclaredMethod("Invoke");

            return invokeMethod?.ReturnType;
        }

        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var invokeMethod = request.ActivationType.GetTypeInfo().GetDeclaredMethod("Invoke");

            var list = new List<Type>(invokeMethod.GetParameters().Select(p => p.ParameterType));
            list.Add(invokeMethod.ReturnType);
            list.Add(request.ActivationType);

            var closedClass = typeof(DelegateExpression<,,,,,,>).MakeGenericType(list.ToArray());

            var closedMethod = closedClass.GetRuntimeMethod("CreateDelegate", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        public class DelegateExpression<T1, T2, T3, T4,T5, TResult, TDelegate>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _arg1Id = Guid.NewGuid().ToString();
            private readonly string _arg2Id = Guid.NewGuid().ToString();
            private readonly string _arg3Id = Guid.NewGuid().ToString();
            private readonly string _arg4Id = Guid.NewGuid().ToString();
            private readonly string _arg5Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;
            private readonly MethodInfo _funcMethodInfo;

            public DelegateExpression(IInjectionScope scope, IActivationExpressionRequest request, IInjectionContextCreator injectionContextCreator)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), request.InjectedType, RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T1), _arg1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T2), _arg2Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T3), _arg3Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T4), _arg4Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T5), _arg5Id));

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);

                _funcMethodInfo = typeof(FuncClass).GetTypeInfo().GetDeclaredMethod("Func");
            }

            private IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request, Type argType, string argId)
            {
                var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod("GetExtraData", new[] { typeof(object) });

                var callExpression = Expression.Call(request.Constants.InjectionContextParameter, getMethod, Expression.Constant(argId));

                return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType));
            }

            public TDelegate CreateDelegate(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                var funcClass = new FuncClass(scope, disposalScope, context, _action, _injectionContextCreator, _arg1Id, _arg2Id, _arg3Id, _arg4Id, _arg5Id);

                return (TDelegate)((object)_funcMethodInfo.CreateDelegate(typeof(TDelegate), funcClass));
            }

            public class FuncClass
            {
                private readonly IExportLocatorScope _scope;
                private readonly IDisposalScope _disposalScope;
                private readonly string _arg1Id;
                private readonly string _arg2Id;
                private readonly string _arg3Id;
                private readonly string _arg4Id;
                private readonly string _arg5Id;
                private readonly IInjectionContext _context;
                private readonly ActivationStrategyDelegate _action;
                private readonly IInjectionContextCreator _injectionContextCreator;

                public FuncClass(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate action, IInjectionContextCreator injectionContextCreator, string arg1Id, string arg2Id, string arg3Id, string arg4Id, string arg5Id)
                {
                    _scope = scope;
                    _disposalScope = disposalScope;
                    _context = context;
                    _action = action;
                    _injectionContextCreator = injectionContextCreator;
                    _arg1Id = arg1Id;
                    _arg2Id = arg2Id;
                    _arg3Id = arg3Id;
                    _arg4Id = arg4Id;
                    _arg5Id = arg5Id;
                }

                public TResult Func(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
                {
                    var newContext = _context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_arg1Id, arg1);
                    newContext.SetExtraData(_arg2Id, arg2);
                    newContext.SetExtraData(_arg3Id, arg3);
                    newContext.SetExtraData(_arg4Id, arg4);
                    newContext.SetExtraData(_arg5Id, arg5);

                    return (TResult)_action(_scope, _disposalScope, newContext);
                }
            }
        }
    }
}
