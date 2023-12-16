﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Strategy for creating Func with 3 args
    /// </summary>
    public class FuncThreeArgWrapperStrategy : BaseWrapperStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public FuncThreeArgWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Func<,,,>), injectionScope)
        {
        }


        /// <summary>
        /// Get the type that is being wrapped
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>wrapped type</returns>
        public override Type GetWrappedType(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof(Func<,,,>) ? type.GetTypeInfo().GenericTypeArguments[3] : null;
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var closedClass = typeof(FuncExpression<,,,>).MakeGenericType(request.ActivationType.GenericTypeArguments);

            var closedMethod = closedClass.GetRuntimeMethod(
                nameof(FuncExpression<object,object,object,object>.CreateFunc), 
                new[] { typeof(IExportLocatorScope), typeof(IDisposalScope), typeof(IInjectionContext), typeof(object) });

            var instance = Activator.CreateInstance(closedClass, scope, request, request.Services.InjectionContextCreator, this);

            request.RequireExportScope();
            request.RequireDisposalScope();

            var callExpression = Expression.Call(
                Expression.Constant(instance), 
                closedMethod, 
                request.ScopeParameter,
                request.DisposalScopeExpression, 
                request.InjectionContextParameter,
                request.Constants.KeyParameter);

            return request.Services.Compiler.CreateNewResult(request, callExpression);
        }

        /// <summary>
        /// Helper class that creates Func with 3 args
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public class FuncExpression<T1, T2, T3, TResult>
        {
            private readonly IInjectionContextCreator _injectionContextCreator;
            private readonly string _t1Id = UniqueStringId.Generate();
            private readonly string _t2Id = UniqueStringId.Generate();
            private readonly string _t3Id = UniqueStringId.Generate();
            private readonly ActivationStrategyDelegate _action;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="request"></param>
            /// <param name="injectionContextCreator"></param>
            /// <param name="activationStrategy"></param>
            public FuncExpression(IInjectionScope scope, IActivationExpressionRequest request,
                IInjectionContextCreator injectionContextCreator, IActivationStrategy activationStrategy)
            {
                _injectionContextCreator = injectionContextCreator;

                var newRequest = request.NewRequest(typeof(TResult), activationStrategy, typeof(Func<T1,T2,T3,TResult>), RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T1), _t1Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T2), _t2Id));
                newRequest.AddKnownValueExpression(CreateKnownValueExpression(request, typeof(T3), _t3Id));

                newRequest.SetLocateKey(request.LocateKey);
                newRequest.DisposalScopeExpression = request.Constants.RootDisposalScope;

                var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

                _action = request.Services.Compiler.CompileDelegate(scope, activationExpression);
            }
            
            /// <summary>
            /// Method creates 3 arg Func
            /// </summary>
            /// <param name="scope"></param>
            /// <param name="disposalScope"></param>
            /// <param name="context"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public Func<T1, T2, T3, TResult> CreateFunc(
                IExportLocatorScope scope, 
                IDisposalScope disposalScope,
                IInjectionContext context,
                object key)
            {
                return (arg1, arg2,arg3) =>
                {
                    var newContext = context?.Clone() ?? _injectionContextCreator.CreateContext(null);

                    newContext.SetExtraData(_t1Id, arg1);
                    newContext.SetExtraData(_t2Id, arg2);
                    newContext.SetExtraData(_t3Id, arg3);

                    return (TResult)_action(scope, disposalScope, newContext, key);
                };
            }
        }
    }
}
