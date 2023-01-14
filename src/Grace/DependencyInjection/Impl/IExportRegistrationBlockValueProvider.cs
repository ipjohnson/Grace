using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Registration block that returns exports, decorators, wrappers and inspectors
    /// </summary>
    public interface IExportRegistrationBlockValueProvider : IExportRegistrationBlock
    {
        /// <summary>
        /// Export strategies from the registration block
        /// </summary>
        IEnumerable<ICompiledExportStrategy> GetExportStrategies();

        /// <summary>
        /// Decorators from the registration block
        /// </summary>
        IEnumerable<ICompiledDecoratorStrategy> GetDecoratorStrategies();

        /// <summary>
        /// Wrappers from the registration block
        /// </summary>
        IEnumerable<ICompiledWrapperStrategy> GetWrapperStrategies();

        /// <summary>
        /// Get inspectors registered in block
        /// </summary>
        IEnumerable<IActivationStrategyInspector> GetInspectors();

        /// <summary>
        /// Get list of missing export strategy providers
        /// </summary>
        IEnumerable<IMissingExportStrategyProvider> GetMissingExportStrategyProviders();

        /// <summary>
        /// Get list of missing dependency expression providers
        /// </summary>
        IEnumerable<IMissingDependencyExpressionProvider> GetMissingDependencyExpressionProviders();

        /// <summary>
        /// Get list of value providers
        /// </summary>
        IEnumerable<IInjectionValueProvider> GetValueProviders();

        /// <summary>
        /// Get member injection selectors
        /// </summary>
        IEnumerable<IMemberInjectionSelector> GetMemberInjectionSelectors();
    }
}
