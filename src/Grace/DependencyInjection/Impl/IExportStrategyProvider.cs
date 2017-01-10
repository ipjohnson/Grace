using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Represents a class that provides export stragies
    /// </summary>
    public interface IExportStrategyProvider
    {
        /// <summary>
        /// Get export strategies
        /// </summary>
        /// <returns>list of exports</returns>
        IEnumerable<ICompiledExportStrategy> ProvideExportStrategies();
    }
}
