using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a collection of strategy collections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActivationStrategyCollectionContainer<T> : IDisposable where T : IActivationStrategy
    {
        /// <summary>
        /// Add strategy to container
        /// </summary>
        /// <param name="strategy">strategy</param>
        void AddStrategy(T strategy);

        /// <summary>
        /// Get all strategies
        /// </summary>
        IEnumerable<T> GetAllStrategies();

        /// <summary>
        /// Get collection for a specific type
        /// </summary>
        /// <param name="type"></param>
        IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type);

        /// <summary>
        /// Get collection for a specific name
        /// </summary>
        /// <param name="name"></param>
        IActivationStrategyCollection<T> GetActivationStrategyCollectionByName(string name);

        /// <summary>
        /// Get all activation types
        /// </summary>
        IEnumerable<Type> GetActivationTypes();

            /// <summary>
        /// Clone the container
        /// </summary>
        IActivationStrategyCollectionContainer<T> Clone();

        /// <summary>
        /// Add strategy inspector
        /// </summary>
        /// <param name="inspector">inspector</param>
        void AddInspector(IActivationStrategyInspector inspector);
    }
}
