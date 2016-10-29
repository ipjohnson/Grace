using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents an export strategy
    /// </summary>
    public interface ICompiledExportStrategy : IConfigurableActivationStrategy, IDecoratorOrExportActivationStrategy, IWrapperOrExportActivationStrategy
    {
        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        IEnumerable<ICompiledExportStrategy> SecondaryStrategies();
    }
}
