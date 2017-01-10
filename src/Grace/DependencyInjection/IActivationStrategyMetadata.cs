using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents metadata that can be assigned to a activation strategy
    /// </summary>
    public interface IActivationStrategyMetadata : IReadOnlyDictionary<object,object>
    {
        /// <summary>
        /// Activation type
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// Exported as 
        /// </summary>
        IEnumerable<Type> ExportAs { get; }

        /// <summary>
        /// Exported as keyed
        /// </summary>
        IEnumerable<KeyValuePair<Type, object>> ExportAsKeyed { get; }

        /// <summary>
        /// Check to see if specific metadata matches
        /// </summary>
        /// <param name="key">key to use</param>
        /// <param name="value">value to compare</param>
        /// <returns></returns>
        bool MetadataMatches(object key, object value);
    }
}
