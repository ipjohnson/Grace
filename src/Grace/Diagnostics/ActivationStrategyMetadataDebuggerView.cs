using System;
using System.Collections.Generic;
using System.Linq;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Metadata debugger view
    /// </summary>
    public class ActivationStrategyMetadataDebuggerView
    {
        private readonly IActivationStrategyMetadata _metadata;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="metadata">metadata</param>
        public ActivationStrategyMetadataDebuggerView(IActivationStrategyMetadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Activation type
        /// </summary>
        public Type ActivationType => _metadata.ActivationType;

        /// <summary>
        /// Export as
        /// </summary>
        public IEnumerable<Type> ExportAs => _metadata.ExportAs;

        /// <summary>
        /// Export as keyed
        /// </summary>
        public IEnumerable<KeyValuePairDebuggerView<Type, object>> ExportAsKeyed
            => _metadata.ExportAsKeyed.Select(kvp => new KeyValuePairDebuggerView<Type, object>(kvp.Key, kvp.Value)).ToList();

        /// <summary>
        /// Data associated with strategy
        /// </summary>
        public IEnumerable<KeyValuePairDebuggerView<object, object>> Data
        {
            get
            {
                var list =
                    _metadata.Select(kvp => new KeyValuePairDebuggerView<object, object>(kvp.Key, kvp.Value)).ToList();

                list.Sort((x, y) => 
                    string.Compare(x.Key?.ToString() ?? "", y.Key?.ToString() ?? "", StringComparison.CurrentCultureIgnoreCase));

                return list;
            }
        }
    }
}
