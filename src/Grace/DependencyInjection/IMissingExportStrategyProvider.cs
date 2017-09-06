using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Provide activation strategies for a missing type
    /// </summary>
    public interface IMissingExportStrategyProvider
    {
        /// <summary>
        /// Can a given request be located using this provider
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
