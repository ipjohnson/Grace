using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class for configurable strategies
    /// </summary>
    public class ConfigurableActivationStrategyDebuggerView
    {
        private readonly IConfigurableActivationStrategy _configurableActivationStrategy;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configurableActivationStrategy"></param>
        public ConfigurableActivationStrategyDebuggerView(IConfigurableActivationStrategy configurableActivationStrategy)
        {
            _configurableActivationStrategy = configurableActivationStrategy;
        }

        /// <summary>
        /// Type being activated
        /// </summary>
        public Type ActivationType => _configurableActivationStrategy.ActivationType;

        /// <summary>
        /// Are entities created externally owned
        /// </summary>
        public bool ExternallyOwned => _configurableActivationStrategy.ExternallyOwned;

        /// <summary>
        /// Does the strategy have conditions
        /// </summary>
        public bool HasConditions => _configurableActivationStrategy.HasConditions;

        /// <summary>
        /// List of dependencies needed for this strategy
        /// </summary>
        public IEnumerable<ActivationStrategyDependency> Dependencies
            => _configurableActivationStrategy.GetDependencies();

        /// <summary>
        /// Export the strategy as types
        /// </summary>
        public IEnumerable<Type> ExportAs
        {
            get
            {
                var exports = _configurableActivationStrategy.ExportAs.ToList();

                exports.Sort((x, y) => string.Compare(x.FullName, y.FullName, StringComparison.CurrentCultureIgnoreCase));

                return exports;
            }
        }

        /// <summary>
        /// Export as keyed type
        /// </summary>
        public IEnumerable<KeyValuePairDebuggerView<Type, object>> ExportAsKeyed
        {
            get
            {
                var list =
                    _configurableActivationStrategy.ExportAsKeyed.Select(
                        kvp => new KeyValuePairDebuggerView<Type, object>(kvp.Key, kvp.Value)).ToList();

                list.Sort((x, y) => string.Compare(x.Key.FullName, y.Key.FullName, StringComparison.CurrentCultureIgnoreCase));

                return list;
            }
        }

        /// <summary>
        /// Metadata
        /// </summary>
        public IActivationStrategyMetadata Metadata => _configurableActivationStrategy.Metadata;

        /// <summary>
        /// Lifestyle for container 
        /// </summary>
        public ICompiledLifestyle Lifestyle => _configurableActivationStrategy.Lifestyle;

        /// <summary>
        /// Priority
        /// </summary>
        public int Priority => _configurableActivationStrategy.Priority;

    }
}
