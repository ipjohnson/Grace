using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IEnrichmentExpressionCreator
    {
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration, IActivationExpressionResult result);
    }

    public class EnrichmentExpressionCreator : IEnrichmentExpressionCreator
    {
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
