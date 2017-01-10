using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Provide activation strategies for a missing type
    /// </summary>
    public interface IMissingExportStrategyProvider
    {
        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
