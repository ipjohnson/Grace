using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Strategy class for creating delegate with no arguements
    /// </summary>
    public class DelegateNoArgWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        public DelegateNoArgWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
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

            var closedClass = typeof(DelegateExpression<,>).MakeGenericType(list.ToArray());

            var closedMethod = closedClass.GetRuntimeMethod(nameof(DelegateExpression<object,object>.CreateDelegate), new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext) });

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
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TDelegate"></typeparam>
        public class DelegateExpression<TResult, TDelegate>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly ActivationStrategyDelegate _action;
            private readonly MethodInfo _funcMethodInfo;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <param name="injectionContextCreator"></param>
            /// <param name="activationStrategy"></param>
            public DelegateExpression(IInjectionScope scope, IActivationExpressionRequest request,
                IInjectionContextCreator injectionContextCreator, IActivationStrategy activationStrategy)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), activationStrategy, typeof(TDelegate), RequestType.Other, null, true);

                newRequest.SetLocateKey(request.LocateKey);
                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);

                _funcMethodInfo = typeof(FuncClass).GetTypeInfo().GetDeclaredMethod(nameof(FuncClass.Func));
            }

            /// <summary>
            /// Method that is called to create delegate
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="disposalScope"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public TDelegate CreateDelegate(IExportLocatorScope scope, IDisposalScope disposalScope,
                IInjectionContext context)
            {
                var funcClass = new FuncClass(scope, disposalScope, context, _action, _injectionContextCreator);

                return (TDelegate)((object)_funcMethodInfo.CreateDelegate(typeof(TDelegate), funcClass));
            }

            /// <summary>
            /// Helper class that has method that is turned into delegate
            /// </summary>
            public class FuncClass
            {
                private readonly IExportLocatorScope _scope;
                private readonly IDisposalScope _disposalScope;
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
                public FuncClass(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context, ActivationStrategyDelegate action, IInjectionContextCreator injectionContextCreator)
                {
                    _scope = scope;
                    _disposalScope = disposalScope;
                    _context = context;
                    _action = action;
                    _injectionContextCreator = injectionContextCreator;
                }

                /// <summary>
                /// Function that is created into delegate
                /// </summary>
                /// <returns></returns>
                public TResult Func()
                {
                    var newContext = _context?.Clone() ?? _injectionContextCreator.CreateContext(null);
                    
                    return (TResult)_action(_scope, _disposalScope, newContext);
                }
            }

        }
    }
}
