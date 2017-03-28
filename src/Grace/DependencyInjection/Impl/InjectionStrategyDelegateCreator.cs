using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Impl.CompiledStrategies;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// provides the logic to create InjectionStrategyDelegate
    /// </summary>
    public interface IInjectionStrategyDelegateCreator
    {
        /// <summary>
        /// Create a new delegate for a given type
        /// </summary>
        /// <param name="scope">scope to use for creating</param>
        /// <param name="locateType">type to inject</param>
        /// <param name="request"></param>
        /// <param name="objectParameter"></param>
        /// <returns></returns>
        IActivationExpressionResult CreateInjectionDelegate(IInjectionScope scope, Type locateType, IActivationExpressionRequest request, ParameterExpression objectParameter);
    }

    /// <summary>
    /// Implementation for creating injection delegate
    /// </summary>
    public class InjectionStrategyDelegateCreator : IInjectionStrategyDelegateCreator
    {
        /// <summary>
        /// Create a new delegate for a given type
        /// </summary>
        /// <param name="scope">scope to use for creating</param>
        /// <param name="locateType">type to inject</param>
        /// <param name="request"></param>
        /// <param name="objectParameter"></param>
        /// <returns></returns>
        public IActivationExpressionResult CreateInjectionDelegate(IInjectionScope scope, Type locateType, IActivationExpressionRequest request, ParameterExpression objectParameter)
        {
            var strategy = new InjectionStrategy(locateType, scope);

            var result = scope.StrategyCompiler.CreateNewResult(request);

            var instanceValue = Expression.Variable(locateType);

            result.AddExtraParameter(instanceValue);
            result.AddExtraExpression(Expression.Assign(instanceValue, Expression.Convert(objectParameter, locateType)));

            var injectPropertyExpressions = CreatePropertyInjectionExpressions(scope, locateType, instanceValue, strategy, request);

            var injectMethodExpressions = CreateMethodInjectionExpressions(scope, locateType, instanceValue, strategy, request);

            foreach (var activationExpressionResult in injectPropertyExpressions)
            {
                result.AddExpressionResult(activationExpressionResult);
            }

            foreach (var activationExpressionResult in injectMethodExpressions)
            {
                result.AddExpressionResult(activationExpressionResult);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="instanceValue"></param>
        /// <param name="strategy"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IActivationExpressionResult> CreatePropertyInjectionExpressions(IInjectionScope scope, Type locateType, ParameterExpression instanceValue, InjectionStrategy strategy, IActivationExpressionRequest request)
        {
            var properties = new Dictionary<string, bool>();

            foreach (var property in locateType.GetRuntimeProperties())
            {
                if (!property.CanWrite || !property.SetMethod.IsPublic || property.SetMethod.IsStatic)
                {
                    continue;
                }

                var attribute = property.GetCustomAttributes(true).FirstOrDefault(a => a is IImportAttribute) as IImportAttribute;

                var importInfo = attribute?.ProvideImportInfo(property.PropertyType, property.Name);

                if (importInfo == null)
                {
                    continue;
                }

                yield return CreatePropertyImportStatement(scope, locateType, instanceValue, strategy, request, property, importInfo);
                
                properties[property.Name] = true;
            }

            var currentScope = scope;

            while (currentScope != null)
            {
                foreach (var selector in currentScope.MemberInjectionSelectors)
                {
                    foreach (var propertyMember in selector.GetPropertiesAndFields(locateType, scope, request))
                    {
                        if (properties.ContainsKey(propertyMember.MemberInfo.Name))
                        {
                            continue;
                        }
                        
                        properties[propertyMember.MemberInfo.Name] = true;
                    }
                }

                currentScope = currentScope.Parent as IInjectionScope;
            }
        }

        private static IActivationExpressionResult CreatePropertyImportStatement(IInjectionScope scope, Type locateType,
            ParameterExpression instanceValue, InjectionStrategy strategy, IActivationExpressionRequest request,
            PropertyInfo property, ImportAttributeInfo importInfo)
        {
            var propertyRequest = request.NewRequest(property.PropertyType, strategy, locateType,
                RequestType.Member, property, false, true);

            propertyRequest.SetIsRequired(importInfo.IsRequired);
            propertyRequest.SetEnumerableComparer(importInfo.Comparer);

            if (importInfo.ImportKey != null)
            {
                propertyRequest.SetLocateKey(importInfo.ImportKey);
            }
            else if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(property.PropertyType))
            {
                propertyRequest.SetLocateKey(property.Name);
            }

            var result = request.Services.ExpressionBuilder.GetActivationExpression(scope, propertyRequest);

            var setExpression = Expression.Assign(Expression.Property(instanceValue, property.SetMethod),
                result.Expression);

            result.AddExtraExpression(setExpression);
            result.Expression = null;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="instanceValue"></param>
        /// <param name="strategy"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IActivationExpressionResult> CreateMethodInjectionExpressions(IInjectionScope scope, Type locateType, ParameterExpression instanceValue, InjectionStrategy strategy, IActivationExpressionRequest request)
        {
            foreach (var method in locateType.GetRuntimeMethods())
            {
                if (!method.IsPublic || method.IsStatic)
                {
                    continue;
                }

                var importAttribute = method.GetCustomAttributes(true).FirstOrDefault(a => a is IImportAttribute) as IImportAttribute;

                var importInfo = importAttribute?.ProvideImportInfo(null, method.Name);

                if (importInfo != null)
                {
                    yield return
                        CreateMethodInjectionExpression(scope, locateType, instanceValue, strategy, request, method, importInfo);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="instanceValue"></param>
        /// <param name="strategy"></param>
        /// <param name="request"></param>
        /// <param name="method"></param>
        /// <param name="importInfo"></param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateMethodInjectionExpression(IInjectionScope scope, Type locateType, ParameterExpression instanceValue, InjectionStrategy strategy, IActivationExpressionRequest request, MethodInfo method, ImportAttributeInfo importInfo)
        {
            var expressionResult = scope.StrategyCompiler.CreateNewResult(request);

            var list = new List<IActivationExpressionResult>();

            foreach (var parameter in method.GetParameters())
            {
                var parameterRequest = request.NewRequest(parameter.ParameterType, strategy, locateType,
                    RequestType.MethodParameter, parameter, false, true);

                if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                {
                    parameterRequest.SetLocateKey(parameter.Name);
                }

                var result = request.Services.ExpressionBuilder.GetActivationExpression(scope, parameterRequest);

                list.Add(result);

                expressionResult.AddExpressionResult(result);
            }

            var callExpression = Expression.Call(instanceValue, method, list.Select(result => result.Expression));

            expressionResult.AddExtraExpression(callExpression);

            return expressionResult;
        }

        /// <summary>
        /// strategy used for requests
        /// </summary>
        public class InjectionStrategy : ConfigurableActivationStrategy
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="activationType">type to activate</param>
            /// <param name="injectionScope">owning injection scope</param>
            public InjectionStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
            {
            }

            /// <summary>
            /// Type of activation strategy
            /// </summary>
            public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.FrameworkExportStrategy;
        }
    }
}
