using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    public interface IMissingExportStrategyProvider
    {
        IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
