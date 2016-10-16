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
    public class DelegateThreeArgWrapperStrategy : BaseWrapperStrategy
    {
        public DelegateThreeArgWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
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

            var closedClass = typeof(DelegateExpression<,,,,>).MakeGenericType(list.ToArray());

            var closedMethod = closedClass.GetRuntimeMethod("CreateDelegate", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator, this);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        public class DelegateExpression<T1, T2, T3, TResult, TDelegate>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _arg1Id = Guid.NewGuid().ToString();
            private readonly string _arg2Id = Guid.NewGuid().ToString();
            private readonly string _arg3Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;
            private readonly MethodInfo _funcMethodInfo;

            public DelegateExpression(IInjectionScope scope, IActivationExpressionRequest request, 
                IInjectionContextCreator injectionContextCreator, IActivationStrategy activationStrategy)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), activationStrategy,typeof(TDelegate), RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T1), _arg1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T2), _arg2Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T3), _arg3Id));

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
                var funcClass = new FuncClass(scope, disposalScope, context, _action, _injectionContextCreator, _arg1Id, _arg2Id, _arg3Id);

                return (TDelegate)((object)_funcMethodInfo.CreateDelegate(typeof(TDelegate), funcClass));
            }

            public class FuncClass
            {
                private readonly IExportLocatorScope _scope;
                private readonly IDisposalScope _disposalScope;
                private readonly string _arg1Id;
                private readonly string _arg2Id;
                private readonly string _arg3Id;
                private readonly IInjectionContext _context;
                private readonly ActivationStrategyDelegate _action;
                private readonly IInjectionContextCreator _injectionContextCreator;

                public FuncClass(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate action, IInjectionContextCreator injectionContextCreator, string arg1Id, string arg2Id, string arg3Id)
                {
                    _scope = scope;
                    _disposalScope = disposalScope;
                    _context = context;
                    _action = action;
                    _injectionContextCreator = injectionContextCreator;
                    _arg1Id = arg1Id;
                    _arg2Id = arg2Id;
                    _arg3Id = arg3Id;
                }

                public TResult Func(T1 arg1, T2 arg2, T3 arg3)
                {
                    var newContext = _context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_arg1Id, arg1);
                    newContext.SetExtraData(_arg2Id, arg2);
                    newContext.SetExtraData(_arg3Id, arg3);

                    return (TResult)_action(_scope, _disposalScope, newContext);
                }
            }
        }
    }
}
