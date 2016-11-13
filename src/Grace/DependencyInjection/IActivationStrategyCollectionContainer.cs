using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a collection of strategy collections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActivationStrategyCollectionContainer<T> where T : IActivationStrategy
    {
        /// <summary>
        /// Add strategy to container
        /// </summary>
        /// <param name="strategy">strategy</param>
        void AddStrategy(T strategy);

        /// <summary>
        /// Get all strategies
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAllStrategies();

        /// <summary>
        /// Get collection for a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type);

        /// <summary>
        /// Get all activation types
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetActivationTypes();

            /// <summary>
        /// Clone the container
        /// </summary>
        /// <returns></returns>
        IActivationStrategyCollectionContainer<T> Clone();

        /// <summary>
        /// Add strategy inspector
        /// </summary>
        /// <param name="inspector">inspector</param>
        void AddInspector(IActivationStrategyInspector inspector);
    }
}
