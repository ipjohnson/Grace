using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates expression to call method on type being instantiated
    /// </summary>
    public interface IMethodInvokeExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request);

        /// <summary>
        /// Create expressions for calling methods
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">configuration information</param>
        /// <param name="activationExpressionResult">expression result</param>
        /// <returns>expression result</returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult activationExpressionResult);
    }

    /// <summary>
    /// Creates expression to call methods on type being instantiated
    /// </summary>
    public class MethodInvokeExpressionCreator : IMethodInvokeExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var list = ImmutableLinkedList<ActivationStrategyDependency>.Empty;

            foreach (var selector in configuration.MemberInjectionSelectors)
            {
                foreach (var methodInjection in selector.GetMethods(configuration.ActivationType, request.RequestingScope, request))
                {
                    list = list.AddRange(GetDependenciesForMethod(methodInjection, configuration, request));
                }
            }

            foreach (var methodInjection in configuration.MethodInjections)
            {
                list = list.AddRange(GetDependenciesForMethod(methodInjection, configuration, request));
            }

            if (configuration.ActivationMethod != null)
            {
                list = list.AddRange(GetDependenciesForMethod(configuration.ActivationMethod, configuration, request));
            }

            return list;
        }


        /// <summary>
        /// Create expressions for calling methods
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">configuration information</param>
        /// <param name="activationExpressionResult">expression result</param>
        /// <returns>expression result</returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult activationExpressionResult)
        {
            ParameterExpression variableExpression = null;

            foreach (var selector in activationConfiguration.MemberInjectionSelectors)
            {
                foreach (var methodInjectionInfo in selector.GetMethods(activationConfiguration.ActivationType, request.RequestingScope, request))
                {
                    if (variableExpression == null)
                    {
                        variableExpression = CreateVariablExpression(activationConfiguration, activationExpressionResult);
                    }

                    AddMethodCall(scope, request, activationConfiguration, activationExpressionResult, methodInjectionInfo, variableExpression);
                }
            }

            foreach (var methodInjectionInfo in activationConfiguration.MethodInjections)
            {
                if (variableExpression == null)
                {
                    variableExpression = CreateVariablExpression(activationConfiguration, activationExpressionResult);
                }

                AddMethodCall(scope, request, activationConfiguration, activationExpressionResult, methodInjectionInfo, variableExpression);
            }

            if (activationConfiguration.ActivationMethod != null)
            {
                if (variableExpression == null)
                {
                    variableExpression = CreateVariablExpression(activationConfiguration, activationExpressionResult);
                }

                AddMethodCall(scope, request, activationConfiguration, activationExpressionResult, activationConfiguration.ActivationMethod, variableExpression);
            }

            return activationExpressionResult;
        }

        private IEnumerable<ActivationStrategyDependency> GetDependenciesForMethod(MethodInjectionInfo methodInjection, TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var list = ImmutableLinkedList<ActivationStrategyDependency>.Empty;

            foreach (var parameter in methodInjection.Method.GetParameters())
            {
                object key = null;

                if (request.RequestingScope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                {
                    key = parameter.Name;
                }

                var found = parameter.ParameterType.IsGenericParameter ||
                            request.RequestingScope.CanLocate(parameter.ParameterType, key: key);

                list =
                    list.Add(new ActivationStrategyDependency(DependencyType.MethodParameter,
                                                              configuration.ActivationStrategy,
                                                              parameter,
                                                              parameter.ParameterType,
                                                              parameter.Name,
                                                              false,
                                                              false,
                                                              found));
            }

            return list;
        }

        private static void AddMethodCall(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration, IActivationExpressionResult activationExpressionResult,
            MethodInjectionInfo methodInjectionInfo, ParameterExpression variableExpression)
        {
            var parameterResults = new List<IActivationExpressionResult>();

            foreach (var parameter in methodInjectionInfo.Method.GetParameters())
            {
                var parameterRequest = request.NewRequest(parameter.ParameterType,
                    activationConfiguration.ActivationStrategy, activationConfiguration.ActivationType,
                    RequestType.MethodParameter, parameter, false, true);

                if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                {
                    parameterRequest.SetLocateKey(parameter.Name);
                }

                var parameterInfo = methodInjectionInfo.ParameterInfos()
                    .Where(p => p.ParameterName == parameter.Name)
                    .FirstOrDefault();

                if (parameterInfo != null)
                {
                    parameterRequest.SetIsRequired(parameterInfo.IsRequired);
                    parameterRequest.SetLocateKey(parameterInfo.LocateKey);
                }

                var result = request.Services.ExpressionBuilder.GetActivationExpression(scope, parameterRequest);

                parameterResults.Add(result);

                activationExpressionResult.AddExpressionResult(result);
            }

            activationExpressionResult.AddExtraExpression(Expression.Call(variableExpression,
                methodInjectionInfo.Method, parameterResults.Select(r => r.Expression)));
        }

        private static ParameterExpression CreateVariablExpression(TypeActivationConfiguration activationConfiguration,
            IActivationExpressionResult activationExpressionResult)
        {
            var variablExpression = Expression.Variable(activationConfiguration.ActivationType);

            activationExpressionResult.AddExtraParameter(variablExpression);

            activationExpressionResult.AddExtraExpression(Expression.Assign(variablExpression,
                activationExpressionResult.Expression));

            activationExpressionResult.Expression = variablExpression;

            return variablExpression;
        }
    }
}
