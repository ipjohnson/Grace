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
        /// <returns></returns>
        IEnumerable<ICompiledExportStrategy> GetExportStrategies();

        /// <summary>
        /// Decorators from the registration block
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICompiledDecoratorStrategy> GetDecoratorStrategies();

        /// <summary>
        /// Wrappers from the registration block
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICompiledWrapperStrategy> GetWrapperStrategies();

        /// <summary>
        /// Get inspectors registered in block
        /// </summary>
        /// <returns></returns>
        IEnumerable<IActivationStrategyInspector> GetInspectors();

        /// <summary>
        /// Get list of missing export strategy providers
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMissingExportStrategyProvider> GetMissingExportStrategyProviders();

        /// <summary>
        /// Get list of missing dependency expression providers
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMissingDependencyExpressionProvider> GetMissingDependencyExpressionProviders();

        /// <summary>
        /// Get list of value providers
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInjectionValueProvider> GetValueProviders();

        /// <summary>
        /// Get member injection selectors
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMemberInjectionSelector> GetMemberInjectionSelectors();
    }
}
