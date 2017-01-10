using System;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Debugger view for injection scope
    /// </summary>
    public class InjectionScopeDebuggerView
    {
        private readonly IInjectionScope _injectionScope;
        private readonly InjectionScopeDiagnostics _diagnostics;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public InjectionScopeDebuggerView(IExportLocatorScope injectionScope)
        {
            _injectionScope = injectionScope.GetInjectionScope();
            _diagnostics = new InjectionScopeDiagnostics(_injectionScope);
        }

        /// <summary>
        /// Scope Configuration
        /// </summary>
        public IInjectionScopeConfiguration Configuration => _injectionScope.ScopeConfiguration;

        /// <summary>
        /// Extra data for scope
        /// </summary>
        public ExtraDataContainerDebuggerView ExtraData => new ExtraDataContainerDebuggerView(_injectionScope);

        /// <summary>
        /// Name of the scope
        /// </summary>
        public string Name => _injectionScope.ScopeName;

        /// <summary>
        /// Scope id
        /// </summary>
        public Guid ScopeId => _injectionScope.ScopeId;

        /// <summary>
        /// Exports in the scope
        /// </summary>
        public CollectionDebuggerView<ICompiledExportStrategy> Exports
            => new CollectionDebuggerView<ICompiledExportStrategy>(_injectionScope.StrategyCollectionContainer.GetAllStrategies());

        /// <summary>
        /// Exports by type
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledExportStrategy> ExportByType
            => _injectionScope.StrategyCollectionContainer;

        /// <summary>
        /// Decorators for the scope
        /// </summary>
        public CollectionDebuggerView<ICompiledDecoratorStrategy> Decorators
            => new CollectionDebuggerView<ICompiledDecoratorStrategy>(_injectionScope.DecoratorCollectionContainer.GetAllStrategies());

        /// <summary>
        /// Wrappers for the scope
        /// </summary>
        public CollectionDebuggerView<ICompiledWrapperStrategy> Wrappers
            => new CollectionDebuggerView<ICompiledWrapperStrategy>(_injectionScope.WrapperCollectionContainer.GetAllStrategies());

        /// <summary>
        /// List of possible missing dependencies in this scope
        /// </summary>
        public CollectionDebuggerView<ActivationStrategyDependency> PossibleMissingDependencies =>
            new CollectionDebuggerView<ActivationStrategyDependency>(_diagnostics.PossibleMissingDependencies);

        /// <summary>
        /// List of exceptions discovered while looking for dependencies
        /// </summary>
        public CollectionDebuggerView<string> ContainerExceptions =>
            new CollectionDebuggerView<string>(_diagnostics.ContainerExceptions);

    }
}
