using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// interface for creating enrichment expressions for activation strategy
    /// </summary>
    public interface IEnrichmentExpressionCreator
    {
        /// <summary>
        /// Create enrichment expressions
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">expression result</param>
        /// <returns></returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    /// <summary>
    /// class for creating enrichment expressions
    /// </summary>
    public class EnrichmentExpressionCreator : IEnrichmentExpressionCreator
    {
        /// <summary>
        /// Create enrichment expressions
        /// </summary>
        /// <param name="scope">scope for strategy</param>
        /// <param name="request">request</param>
        /// <param name="activationConfiguration">activation configuration</param>
        /// <param name="result">expression result</param>
        /// <returns></returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result)
        {
            var expression = result.Expression;
           
            foreach (var enrichmentDelegate in activationConfiguration.EnrichmentDelegates.Reverse())
            {
                var invokeMethod = enrichmentDelegate.GetType().GetRuntimeMethods().First(m => m.Name == "Invoke");

                var expressions = new List<Expression>();

                foreach (var parameter in invokeMethod.GetParameters())
                {
                    if (parameter.ParameterType.GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
                    {
                        expressions.Add(expression);
                    }
                    else
                    {
                        var arg1Request = request.NewRequest(parameter.ParameterType, activationConfiguration.ActivationStrategy, expression.Type, RequestType.Other, null, true, true);

                        var activationExpression = request.Services.ExpressionBuilder.GetActivationExpression(scope, arg1Request);

                        result.AddExpressionResult(activationExpression);

                        expressions.Add(activationExpression.Expression);
                    }
                }

                expression = Expression.Call(Expression.Constant(enrichmentDelegate), invokeMethod,
                    expressions);

                if (activationConfiguration.ActivationType != expression.Type)
                {
                    expression = Expression.Convert(expression, activationConfiguration.ActivationType);
                }
            }

            result.Expression = expression;

            return result;
        }
    }
}
