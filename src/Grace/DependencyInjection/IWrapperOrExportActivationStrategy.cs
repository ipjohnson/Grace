using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a wrapper strategy or an export strategy
    /// </summary>
    public interface IWrapperOrExportActivationStrategy : IActivationStrategy
    {
        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <param name="key">key of keyed activation</param>
        /// <returns>activation delegate</returns>
        ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope, 
            IActivationStrategyCompiler compiler, 
            Type activationType,
            object key = null);

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
