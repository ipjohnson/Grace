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

                expression = Expression.Call(Expression.Constant(enrichmentDelegate), invokeMethod,
                    request.Constants.ScopeParameter, expression);
            }

            result.Expression = expression;

            return result;
        }
    }
}
