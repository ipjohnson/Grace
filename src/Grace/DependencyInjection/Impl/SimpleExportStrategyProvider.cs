using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Simple class to return list of export strategies
    /// </summary>
    public class SimpleExportStrategyProvider : IExportStrategyProvider
    {
        private readonly ICompiledExportStrategy[] _exportStrategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportStrategy"></param>
        public SimpleExportStrategyProvider(params ICompiledExportStrategy[] exportStrategy)
        {
            _exportStrategy = exportStrategy;
        }


        /// <summary>
        /// Get export strategies
        /// </summary>
        /// <returns>list of exports</returns>
        public IEnumerable<ICompiledExportStrategy> ProvideExportStrategies()
        {
            return _exportStrategy;
        }
    }
}
