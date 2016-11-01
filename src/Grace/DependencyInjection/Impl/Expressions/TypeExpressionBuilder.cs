using System;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface ITypeExpressionBuilder
    {
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration);
    }

    public class TypeExpressionBuilder : ITypeExpressionBuilder
    {
        private readonly IInstantiationExpressionCreator _instantiationExpressionCreator;
        private readonly IDisposalScopeExpressionCreator _disposalScopeExpressionCreator;
        private readonly IMemberInjectionExpressionCreator _memberInjectionExpressionCreator;
        private readonly IEnrichmentExpressionCreator _enrichmentExpressionCreator;

        public TypeExpressionBuilder(IInstantiationExpressionCreator instantiationExpressionCreator,
                                     IDisposalScopeExpressionCreator disposalScopeExpressionCreator, 
                                     IMemberInjectionExpressionCreator memberInjectionExpressionCreator, 
                                     IEnrichmentExpressionCreator enrichmentExpressionCreator)
        {
            _instantiationExpressionCreator = instantiationExpressionCreator;
            _disposalScopeExpressionCreator = disposalScopeExpressionCreator;
            _memberInjectionExpressionCreator = memberInjectionExpressionCreator;
            _enrichmentExpressionCreator = enrichmentExpressionCreator;
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            var currentExpression = _instantiationExpressionCreator.CreateExpression(scope, request, activationConfiguration);

            currentExpression = _memberInjectionExpressionCreator.CreateExpression(scope, request, activationConfiguration, currentExpression);
            
            if (activationConfiguration.ActivationType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDisposable)) && 
                activationConfiguration.ExternallyOwned == false)
            {
                currentExpression = _disposalScopeExpressionCreator.CreateExpression(scope, request,
                    activationConfiguration, currentExpression);
            }

            currentExpression = _enrichmentExpressionCreator.CreateExpression(scope, request, activationConfiguration, currentExpression);

            return currentExpression;
        }
    }
}
