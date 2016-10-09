using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
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
    }
}
