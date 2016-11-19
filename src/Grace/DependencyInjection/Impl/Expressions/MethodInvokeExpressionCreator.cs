using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Creates expression to call method on type being instantiated
    /// </summary>
    public interface IMethodInvokeExpressionCreator
    {
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

        private static void AddMethodCall(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration, IActivationExpressionResult activationExpressionResult,
            MethodInjectionInfo methodInjectionInfo, ParameterExpression variableExpression)
        {
            List<IActivationExpressionResult> parameterResults = new List<IActivationExpressionResult>();

            foreach (var parameter in methodInjectionInfo.Method.GetParameters())
            {
                var parameterRequest = request.NewRequest(parameter.ParameterType,
                    activationConfiguration.ActivationStrategy, activationConfiguration.ActivationType,
                    RequestType.MethodParameter, parameter);

                if (scope.ScopeConfiguration.Behaviors.KeyedTypeSelector(parameter.ParameterType))
                {
                    parameterRequest.SetLocateKey(parameter.Name);
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
