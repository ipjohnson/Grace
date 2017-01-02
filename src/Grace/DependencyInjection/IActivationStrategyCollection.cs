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
        /// <returns></returns>
        T GetPrimary();

        /// <summary>
        /// Strategies that are non keyed
        /// </summary>
        /// <returns></returns>
        ImmutableArray<T> GetStrategies();

        /// <summary>
        /// list of strategies and their keys
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies();

        /// <summary>
        /// Get a keyed strategy
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        T GetKeyedStrategy(object key);

        /// <summary>
        /// Clone the collection
        /// </summary>
        /// <returns></returns>
        IActivationStrategyCollection<T> Clone();
    }
}
