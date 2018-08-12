using System;
using System.Collections.Generic;
using System.Text;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Compiled wrapper that wrap keyed services.
    /// </summary>
    public interface IKeyWrapperActivationStrategy : ICompiledWrapperStrategy
    {
        /// <summary>
        /// Get an activation strategy for this delegate
        /// </summary>
        /// <param name="scope">injection scope</param>
        /// <param name="compiler"></param>
        /// <param name="activationType">activation type</param>
        /// <param name="key">The locate key.</param>
        /// <returns>activation delegate</returns>
        ActivationStrategyDelegate GetActivationStrategyDelegate(
            IInjectionScope scope, IActivationStrategyCompiler compiler, Type activationType, object key);

    }
}
