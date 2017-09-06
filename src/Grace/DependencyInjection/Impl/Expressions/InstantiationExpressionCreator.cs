using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl.Expressions
{
    /// <summary>
    /// implementation for creating an instantiation expression for a type
    /// </summary>
    public interface IInstantiationExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration,
            IActivationExpressionRequest request);

        /// <summary>
        /// Create instantiation expression
        /// </summary>
        /// <param name="scope">scope the configuration is associated with</param>
        /// <param name="request">expression request</param>
        /// <param name="activationConfiguration">configuration</param>
        /// <returns>expression result</returns>
        IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration activationConfiguration);
    }

    /// <summary>
    /// Creates instantiation expressions
    /// </summary>
    public class InstantiationExpressionCreator : IInstantiationExpressionCreator
    {
        /// <summary>
        /// Get an enumeration of dependencies
        /// </summary>
        /// <param name="configuration">configuration object</param>
        /// <param name="request"></param>
        /// <returns>dependencies</returns>
        public IEnumerable<ActivationStrategyDependency> GetDependencies(TypeActivationConfiguration configuration, IActivationExpressionRequest request)
        {
            var creator = configuration.ConstructorSelectionMethod ??
                                  request.RequestingScope.ScopeConfiguration.Behaviors.ConstructorSelection;
            
            return creator.GetDependencies(configuration, request);
        }

        /// <summary>
        /// Create instantiation expression
        /// </summary>
        /// <param name="scope">scope the configuration is associated with</param>
        /// <param name="request">expression request</param>
        /// <param name="configuration">configuration</param>
        /// <returns>expression result</returns>
        public IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            TypeActivationConfiguration configuration)
        {
            var creator = configuration.ConstructorSelectionMethod ??
                          request.RequestingScope.ScopeConfiguration.Behaviors.ConstructorSelection;

            return creator.CreateExpression(scope, request, configuration);
        }
        
    }
}
