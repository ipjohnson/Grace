using System;

namespace Grace.DependencyInjection
{
    public interface IWrapperOrExportActivationStrategy : IActivationStrategy
    {
        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <returns>activation delegate</returns>
        ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType);

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);
    }
}
