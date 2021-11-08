using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// Builds expression using type activation configuration
    /// </summary>
    public interface ITypeExpressionBuilder
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request);

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
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var returnList = ImmutableLinkedList<ActivationStrategyDependency>.Empty;

            returnList = returnList.AddRange(_methodInvokeExpressionCreator.GetDependencies(configuration, request));
            returnList = returnList.AddRange(_memberInjectionExpressionCreator.GetDependencies(configuration, request));
            returnList = returnList.AddRange(_instantiationExpressionCreator.GetDependencies(configuration, request));

            return returnList;
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

            if (ShouldTrackForDisposable(scope, activationConfiguration))
            {
                currentExpression = _disposalScopeExpressionCreator.CreateExpression(scope, request,
                    activationConfiguration, currentExpression);
            }

            currentExpression = _enrichmentExpressionCreator.CreateExpression(scope, request, activationConfiguration, currentExpression);

            return currentExpression;
        }

        private bool ShouldTrackForDisposable(IInjectionScope scope, TypeActivationConfiguration activationConfiguration)
        {
            var implementedInterfaces = activationConfiguration.ActivationType.GetTypeInfo().ImplementedInterfaces;

#if NETSTANDARD2_1
            if (!implementedInterfaces.Contains(typeof(IAsyncDisposable)) && !implementedInterfaces.Contains(typeof(IDisposable)))
            {
                return false;
            }
#else
            if (implementedInterfaces.Contains(typeof(IDisposable)) == false)
            {
                return false;
            }
#endif
            if (activationConfiguration.ExternallyOwned)
            {
                return false;
            }

            return scope.ScopeConfiguration.TrackDisposableTransients ||
                 (activationConfiguration.Lifestyle != null && 
                  activationConfiguration.Lifestyle.LifestyleType != LifestyleType.Transient);
        }
    }
}
