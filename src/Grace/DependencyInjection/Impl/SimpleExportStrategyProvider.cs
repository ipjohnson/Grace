using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
    public class SimpleExportStrategyProvider : IExportStrategyProvider
    {
        private readonly ICompiledExportStrategy[] _exportStrategy;

        public SimpleExportStrategyProvider(params ICompiledExportStrategy[] exportStrategy)
        {
            _exportStrategy = exportStrategy;
        }

        public IEnumerable<ICompiledExportStrategy> ProvideExportStrategies()
        {
            return _exportStrategy;
        }
    }
}
