using System;
using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a collection of activation strategies
    /// </summary>
    /// <typeparam name="T">strategy type</typeparam>
    public interface IActivationStrategyCollection<T> : IDisposable where T : IActivationStrategy
    {
        /// <summary>
        /// Add a new strategy to collection
        /// </summary>
        /// <param name="strategy">strategy to add</param>
        /// <param name="key">key associated with type, can be null</param>
        void AddStrategy(T strategy, object key);

        /// <summary>
        /// Get primary strategy
        /// </summary>
        T GetPrimary();

        /// <summary>
        /// Strategies that are non keyed
        /// </summary>
        ImmutableArray<T> GetStrategies();

        /// <summary>
        /// List of all keyed strategies, with their key
        /// </summary>
        IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies();

        /// <summary>
        /// Get all strategies for a given key (incl. ImportKey.Any)
        /// </summary>
        /// <param name="key">key</param>
        IEnumerable<T> GetKeyedStrategies(object key);

        /// <summary>
        /// Get main keyed strategy for a given key (fallbacks to ImportKey.Any)
        /// </summary>
        /// <param name="key">key</param>
        T GetKeyedStrategy(object key);

        /// <summary>
        /// Clone the collection
        /// </summary>
        IActivationStrategyCollection<T> Clone();
    }
}
