using System;
using System.Collections.Generic;
using System.Linq;
using Grace.Data.Immutable;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class for injection scope
    /// </summary>
    public class InjectionScopeDiagnostics
    {
        private readonly IInjectionScope _injectionScope;
        private ImmutableLinkedList<ActivationStrategyDependency> _missingDependencies = ImmutableLinkedList<ActivationStrategyDependency>.Empty;
        private ImmutableLinkedList<string> _containerExceptions = ImmutableLinkedList<string>.Empty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        public InjectionScopeDiagnostics(IExportLocatorScope injectionScope)
        {
            _injectionScope = injectionScope.GetInjectionScope();

            ProcessInjectionDependencies();
        }
        
        /// <summary>
        /// List of possible missing dependencies in this scope
        /// </summary>
        public IEnumerable<ActivationStrategyDependency> PossibleMissingDependencies => _missingDependencies;

        /// <summary>
        /// List of exceptions discovered while looking for dependencies
        /// </summary>
        public IEnumerable<string> ContainerExceptions => _containerExceptions;

        private void ProcessInjectionDependencies()
        {
            foreach (var strategy in _injectionScope.StrategyCollectionContainer.GetAllStrategies())
            {
                try
                {
                    var dependencies = strategy.GetDependencies();

                    _missingDependencies = _missingDependencies.AddRange(dependencies.Where(d => d.IsSatisfied == false));
                }
                catch (Exception exp)
                {
                    _containerExceptions = _containerExceptions.Add(exp.Message);
                }
            }
        }
    }
}
