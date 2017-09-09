using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Impl.InstanceStrategies;
using Grace.DependencyInjection.Lifestyle;
using Grace.Utilities;

namespace Grace.Factory.Impl
{
    /// <summary>
    /// Creates a dynamic factory class for interface type
    /// </summary>
    public class DynamicFactoryStrategy : BaseInstanceExportStrategy
    {
        private Type _proxyType;
        private List<DynamicTypeBuilder.DelegateInfo> _delegateInfo;
        private readonly object _proxyTypeLock = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType"></param>
        /// <param name="injectionScope"></param>
        public DynamicFactoryStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {

        }

        /// <inheritdoc />
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            if (_proxyType == null)
            {
                lock (_proxyTypeLock)
                {
                    if (_proxyType == null)
                    {
                        var builder = new DynamicTypeBuilder();

                        _proxyType = builder.CreateType(ActivationType, out _delegateInfo);
                    }
                }
            }

            var parameters = new List<Expression>
            {
                request.ScopeParameter,
                request.DisposalScopeExpression,
                request.InjectionContextParameter
            };

            var uniqueId = UniqueStringId.Generate();

            foreach (var delegateInfo in _delegateInfo)
            {
                var locateType = delegateInfo.Method.ReturnType;

                var newRequest = request.NewRequest(locateType, this, ActivationType, RequestType.Other, null, true);

                newRequest.AddKnownValueExpression(
                    CreateKnownValueExpression(newRequest, ActivationType, uniqueId));
                
                if (delegateInfo.Method.Name.StartsWith("Get"))
                {
                    newRequest.SetLocateKey(delegateInfo.Method.Name.Substring("Get".Length));
                }

                if (delegateInfo.ParameterInfos != null)
                {
                    foreach (var parameter in delegateInfo.ParameterInfos)
                    {
                        newRequest.AddKnownValueExpression(
                            CreateKnownValueExpression(newRequest, parameter.ParameterInfo.ParameterType, parameter.UniqueId, parameter.ParameterInfo.Name, parameter.ParameterInfo.Position));
                    }
                }

                var result = request.Services.ExpressionBuilder.GetActivationExpression(request.RequestingScope, newRequest);

                var compiledDelegate = request.Services.Compiler.CompileDelegate(request.RequestingScope, result);

                parameters.Add(Expression.Constant(compiledDelegate));
            }
            
            var constructor = _proxyType.GetTypeInfo().DeclaredConstructors.First();

            request.RequireInjectionContext();

            var newStatement = Expression.New(constructor, parameters);

            var setMethod = typeof(IExtraDataContainer).GetRuntimeMethod("SetExtraData",
                new[] { typeof(object), typeof(object), typeof(bool) });

            var invokeStatement = Expression.Call(request.InjectionContextParameter, setMethod,
                Expression.Constant(uniqueId), newStatement, Expression.Constant(true));

            var castStatement = Expression.Convert(invokeStatement, ActivationType);

            return request.Services.Compiler.CreateNewResult(request, castStatement);
        }

        private IKnownValueExpression CreateKnownValueExpression(IActivationExpressionRequest request, Type argType, string valueId, string nameHint = null, int? position = null)
        {
            var getMethod = typeof(IExtraDataContainer).GetRuntimeMethod("GetExtraData", new[] { typeof(object) });

            var callExpression = Expression.Call(request.InjectionContextParameter, getMethod,
                Expression.Constant(valueId));

            return new SimpleKnownValueExpression(argType, Expression.Convert(callExpression, argType), nameHint, position);
        }
    }
}
