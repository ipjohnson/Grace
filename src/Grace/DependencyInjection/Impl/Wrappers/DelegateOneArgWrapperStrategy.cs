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
    public class DelegateOneArgWrapperStrategy : BaseWrapperStrategy
    {
        
        public DelegateOneArgWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
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

            var closedClass = typeof(DelegateExpression<,,>).MakeGenericType(list.ToArray());

            var closedMethod = closedClass.GetRuntimeMethod("CreateDelegate", new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator);

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.Constants.ScopeParameter,
                    request.DisposalScopeExpression, request.Constants.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        public class DelegateExpression<TArg1, TResult, TDelegate>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _arg1Id = Guid.NewGuid().ToString();
            private readonly ActivationStrategyDelegate _action;
            private readonly MethodInfo _funcMethodInfo;

            public DelegateExpression(IInjectionScope scope, IActivationExpressionRequest request, IInjectionContextCreator injectionContextCreator)
            {
                _injectionContextCreator = injectionContextCreator;
                
                var newRequest = request.NewRequest(typeof(TResult), request.InjectedType, RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request));

                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);

                _funcMethodInfo = typeof(FuncClass).GetTypeInfo().GetDeclaredMethod("Func");
            }

            private IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request)
            {
                var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod("GetExtraData", new[] { typeof(object) });

                var argType = typeof(TArg1);

                var callExpression = Expression.Call(request.Constants.InjectionContextParameter, getMethod, Expression.Constant(_arg1Id));

                return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType));
            }

            public TDelegate CreateDelegate(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                var funcClass = new FuncClass(scope, disposalScope, context, _action, _injectionContextCreator, _arg1Id);

                return (TDelegate)((object)_funcMethodInfo.CreateDelegate(typeof(TDelegate), funcClass));
            }

            public class FuncClass
            {
                private readonly IExportLocatorScope _scope;
                private readonly IDisposalScope _disposalScope;
                private readonly string _arg1Id;
                private readonly IInjectionContext _context;
                private readonly ActivationStrategyDelegate _action;
                private readonly IInjectionContextCreator _injectionContextCreator;

                public FuncClass(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate action, IInjectionContextCreator injectionContextCreator, string arg1Id)
                {
                    _scope = scope;
                    _disposalScope = disposalScope;
                    _context = context;
                    _action = action;
                    _injectionContextCreator = injectionContextCreator;
                    _arg1Id = arg1Id;
                }

                public TResult Func(TArg1 arg)
                {
                    var newContext = _context?.Clone() ?? _injectionContextCreator.CreateContext(typeof(TResult), null);

                    newContext.SetExtraData(_arg1Id, arg);

                    return (TResult)_action(_scope, _disposalScope, newContext);
                }
            }

        }
    }
}
