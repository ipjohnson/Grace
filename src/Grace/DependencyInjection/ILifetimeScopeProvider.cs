using System;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Interface for providing IExportLocatorScope instances
    /// </summary>
    public interface ILifetimeScopeProvider
    {
        /// <summary>
        /// Create new scope 
        /// </summary>
        /// <param name="parentScope"></param>
        /// <param name="scopeName"></param>
        /// <param name="activationStrategyDelegates"></param>
        /// <returns></returns>
        IExportLocatorScope CreateScope(IExportLocatorScope parentScope, string scopeName, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationStrategyDelegates);
    }
}
