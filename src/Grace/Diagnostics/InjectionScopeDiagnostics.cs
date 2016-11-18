using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class for injection scope
    /// </summary>
    public class InjectionScopeDiagnostics
    {
        private readonly IInjectionScope _injectionScope;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public InjectionScopeDiagnostics(IExportLocatorScope injectionScope)
        {
            _injectionScope = injectionScope.GetInjectionScope();
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
        public string Name => _injectionScope.Name;

        /// <summary>
        /// Scope id
        /// </summary>
        public Guid ScopeId => _injectionScope.ScopeId;
        
        /// <summary>
        /// Exports in the scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledExportStrategy> Exports => _injectionScope.StrategyCollectionContainer;

        /// <summary>
        /// Decorators for the scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledDecoratorStrategy> Decorators => _injectionScope.DecoratorCollectionContainer;

        /// <summary>
        /// Wrappers for the scope
        /// </summary>
        public IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> Wrappers => _injectionScope.WrapperCollectionContainer;

        /// <summary>
        /// Missing export strategy provides
        /// </summary>
        public IEnumerable<IMissingExportStrategyProvider> MissingExportStrategyProviders
            => _injectionScope.MissingExportStrategyProviders;
    }
}
