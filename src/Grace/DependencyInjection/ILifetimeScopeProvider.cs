using System;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    public interface ILifetimeScopeProvider
    {
        IExportLocatorScope CreateScope(RootInjectionScope rootInjectionScope, string scopeName, ImmutableHashTree<Type, ActivationStrategyDelegate>[] activationStrategyDelegates);
    }
}
