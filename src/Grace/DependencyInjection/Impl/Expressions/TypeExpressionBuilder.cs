using System;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Builds expression using type activation configuration
    /// </summary>
    public interface ITypeExpressionBuilder
    {
        /// <summary>
        /// Get activation expression
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request for expression</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>result</returns>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, TypeActivationConfiguration activationConfiguration);
    }

    /// <summary>
    /// expression builder
    /// </summary>
    public class TypeExpressionBuilder : ITypeExpressionBuilder
    {
        private readonly IInstantiationExpressionCreator _instantiationExpressionCreator;
        private readonly IDisposalScopeExpressionCreator _disposalScopeExpressionCreator;
        private readonly IMemberInjectionExpressionCreator _memberInjectionExpressionCreator;
        private readonly IMethodInvokeExpressionCreator _methodInvokeExpressionCreator;
        private readonly IEnrichmentExpressionCreator _enrichmentExpressionCreator;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="instantiationExpressionCreator"></param>
        /// <param name="disposalScopeExpressionCreator"></param>
        /// <param name="memberInjectionExpressionCreator"></param>
        /// <param name="enrichmentExpressionCreator"></param>
        /// <param name="methodInvokeExpressionCreator"></param>
        public TypeExpressionBuilder(IInstantiationExpressionCreator instantiationExpressionCreator,
                                     IDisposalScopeExpressionCreator disposalScopeExpressionCreator, 
                                     IMemberInjectionExpressionCreator memberInjectionExpressionCreator, 
                                     IMethodInvokeExpressionCreator methodInvokeExpressionCreator,
                                     IEnrichmentExpressionCreator enrichmentExpressionCreator)
        {
            _instantiationExpressionCreator = instantiationExpressionCreator;
            _disposalScopeExpressionCreator = disposalScopeExpressionCreator;
            _memberInjectionExpressionCreator = memberInjectionExpressionCreator;
            _enrichmentExpressionCreator = enrichmentExpressionCreator;
            _methodInvokeExpressionCreator = methodInvokeExpressionCreator;
        }

        /// <summary>
        /// Get activation expression
        /// </summary>
        /// <param name="scope">scope</param>
        /// <param name="request">request for expression</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>result</returns>
        public virtual IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration)
        {
            var currentExpression = _instantiationExpressionCreator.CreateExpression(scope, request, activationConfiguration);

            currentExpression = _memberInjectionExpressionCreator.CreateExpression(scope, request, activationConfiguration, currentExpression);
            
            currentExpression = _methodInvokeExpressionCreator.CreateExpression(scope, request, activationConfiguration, currentExpression);

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
