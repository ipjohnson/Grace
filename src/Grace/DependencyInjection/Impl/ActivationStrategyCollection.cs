using System;
using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// holds a set of activation strategies
    /// </summary>
    /// <typeparam name="T">type of activation strategy</typeparam>
    public class ActivationStrategyCollection<T> : IActivationStrategyCollection<T> where T : class, IActivationStrategy
    {
        private readonly Type _exportType;
        private T _primary;
        private bool _hasPriorities;
        private bool _hasConditions;
        private ImmutableArray<T> _strategies = ImmutableArray<T>.Empty;
        private ImmutableHashTree<object, T> _keyedStrategies = ImmutableHashTree<object, T>.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="exportType"></param>
        public ActivationStrategyCollection(Type exportType)
        {
            _exportType = exportType;
        }

        /// <summary>
        /// Add a new strategy to collection
        /// </summary>
        /// <param name="strategy">strategy to add</param>
        /// <param name="key">key associated with type, can be null</param>
        public void AddStrategy(T strategy, object key)
        {
            if (key == null)
            {
                if (_hasPriorities || strategy.Priority != 0)
                {
                    var index = 0;

                    for (; index < _strategies.Length; index++)
                    {
                        if (strategy.Priority > _strategies[index].Priority)
                        {
                            break;
                        }
                    }

                    _strategies = _strategies.Insert(index, strategy);

                    _hasConditions = _hasConditions || strategy.HasConditions;

                    _primary = !_hasConditions ? _strategies[0] : null;

                    _hasPriorities = true;
                }
                else
                {
                    _strategies = _strategies.Add(strategy);

                    _hasConditions = _hasConditions || strategy.HasConditions;

                    _primary = !_hasConditions ? strategy : null;
                }
            }
            else
            {
                _keyedStrategies = _keyedStrategies.Add(key, strategy, (o, n) => n);
            }
        }

        /// <summary>
        /// Strategies that are non keyed
        /// </summary>
        /// <returns></returns>
        public ImmutableArray<T> GetStrategies()
        {
            return _strategies;
        }

        /// <summary>
        /// Get primary strategy
        /// </summary>
        /// <returns></returns>
        public T GetPrimary()
        {
            return _primary;
        }

        /// <summary>
        /// list of strategies and their keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies()
        {
            return _keyedStrategies;
        }

        /// <summary>
        /// Get a keyed strategy
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public T GetKeyedStrategy(object key)
        {
            return _keyedStrategies.GetValueOrDefault(key);
        }

        /// <summary>
        /// Clone the collection
        /// </summary>
        /// <returns></returns>
        public IActivationStrategyCollection<T> Clone()
        {
            return new ActivationStrategyCollection<T>(_exportType)
            {
                _strategies = _strategies,
                _hasConditions = _hasConditions,
                _hasPriorities = _hasPriorities,
                _keyedStrategies = _keyedStrategies,
                _primary = _primary
            };
        }

        /// <summary>
        /// Dispose of collection
        /// </summary>
        public void Dispose()
        {
            var strategies = _strategies;
            var keyedStrategies = _keyedStrategies;

            _primary = null;
            _strategies = ImmutableArray<T>.Empty;
            _keyedStrategies = ImmutableHashTree<object, T>.Empty;

            foreach (var strategy in strategies)
            {
                strategy.Dispose();
            }

            foreach (var keyedStrategy in keyedStrategies)
            {
                keyedStrategy.Value.Dispose();
            }
        }
    }
}
