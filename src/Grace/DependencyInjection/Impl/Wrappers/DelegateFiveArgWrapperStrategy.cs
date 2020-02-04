using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Strategy for creating a delegate with five arguements
    /// </summary>
    public class DelegateFiveArgWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">activation type</param>
        /// <param name="injectionScope">injection scope</param>
        public DelegateFiveArgWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {
        }

        /// <summary>
        /// Get the type that is being wrapped
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>wrapped type</returns>
        public override Type GetWrappedType(Type type)
        {
            var invokeMethod = type.GetTypeInfo().GetDeclaredMethod("Invoke");

            return invokeMethod?.ReturnType;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var invokeMethod = request.ActivationType.GetTypeInfo().GetDeclaredMethod("Invoke");

            var list = new List<Type>(invokeMethod.GetParameters().Select(p => p.ParameterType));
            list.Add(invokeMethod.ReturnType);
            list.Add(request.ActivationType);

            var closedClass = typeof(DelegateExpression<,,,,,,>).MakeGenericType(list.ToArray());

            var closedMethod = closedClass.GetRuntimeMethod(nameof(DelegateExpression<object,object,object,object,object,object,object>.CreateDelegate), new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator, this);

            request.RequireExportScope();
            request.RequireDisposalScope();

            var callExpression =
                Expression.Call(Expression.Constant(instance), closedMethod, request.ScopeParameter,
                    request.DisposalScopeExpression, request.InjectionContextParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        /// <summary>
        /// Helper class that creates delegate expression
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        public class DelegateExpression<T1, T2, T3, T4,T5, TResult, TDelegate>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _arg1Id = UniqueStringId.Generate();
            private readonly string _arg2Id = UniqueStringId.Generate();
            private readonly string _arg3Id = UniqueStringId.Generate();
            private readonly string _arg4Id = UniqueStringId.Generate();
            private readonly string _arg5Id = UniqueStringId.Generate();
            private readonly ActivationStrategyDelegate _action;
            private readonly MethodInfo _funcMethodInfo;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope">scope</param>
            /// <param name="request">request</param>
            /// <param name="injectionContextCreator">injection context creator</param>
            /// <param name="strategy">strategy</param>
            public DelegateExpression(IInjectionScope scope, IActivationExpressionRequest request, IInjectionContextCreator injectionContextCreator, IActivationStrategy strategy)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), strategy, typeof(TDelegate), RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T1), _arg1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T2), _arg2Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T3), _arg3Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T4), _arg4Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T5), _arg5Id));

                newRequest.SetLocateKey(request.LocateKey);
                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);

                _funcMethodInfo = typeof(FuncClass).GetTypeInfo().GetDeclaredMethod(nameof(FuncClass.Func));
            }
            
            /// <summary>
            /// Method that is called each time a delegate needs to be created
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="disposalScope"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public TDelegate CreateDelegate(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                var funcClass = new FuncClass(scope, disposalScope, context, _action, _injectionContextCreator, _arg1Id, _arg2Id, _arg3Id, _arg4Id, _arg5Id);

                return (TDelegate)((object)_funcMethodInfo.CreateDelegate(typeof(TDelegate), funcClass));
            }

            /// <summary>
            /// Helper class that provides method that is turned into delegate
            /// </summary>
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

                /// <summary>
                /// Default constructor
                /// </summary>
                /// <param name="scope"></param>
                /// <param name="disposalScope"></param>
                /// <param name="context"></param>
                /// <param name="action"></param>
                /// <param name="injectionContextCreator"></param>
                /// <param name="arg1Id"></param>
                /// <param name="arg2Id"></param>
                /// <param name="arg3Id"></param>
                /// <param name="arg4Id"></param>
                /// <param name="arg5Id"></param>
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

                /// <summary>
                /// Method invoked when delegate is executed
                /// </summary>
                /// <param name="arg1"></param>
                /// <param name="arg2"></param>
                /// <param name="arg3"></param>
                /// <param name="arg4"></param>
                /// <param name="arg5"></param>
                /// <returns></returns>
                public TResult Func(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
                {
                    var newContext = _context?.Clone() ?? _injectionContextCreator.CreateContext(null);

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
