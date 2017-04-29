using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents an export strategy
    /// </summary>
    public interface ICompiledExportStrategy : IConfigurableActivationStrategy, IDecoratorOrExportActivationStrategy, IWrapperOrExportActivationStrategy
    {
        /// <summary>
        /// Add a secondary strategy for this export strategy
        /// </summary>
        /// <param name="secondaryStrategy">new secondary strategy</param>
        void AddSecondaryStrategy(ICompiledExportStrategy secondaryStrategy);

        /// <summary>
        /// Provide secondary strategies such as exporting property or method
        /// </summary>
        /// <returns>export strategies</returns>
        IEnumerable<ICompiledExportStrategy> SecondaryStrategies();

    }
}
