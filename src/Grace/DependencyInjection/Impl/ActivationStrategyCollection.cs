using System;
using System.Collections.Generic;
using System.Linq;
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
        private ImmutableHashTree<object, ImmutableArray<T>> _keyedStrategies 
            = ImmutableHashTree<object, ImmutableArray<T>>.Empty;

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
                _keyedStrategies = _keyedStrategies.Add(
                    key, 
                    ImmutableArray.Create(strategy),
                    (o, n) => o.AddRange(n));
            }
        }

        /// <summary>
        /// Strategies that are non keyed
        /// </summary>
        public ImmutableArray<T> GetStrategies()
        {
            return _strategies;
        }

        /// <summary>
        /// Get primary strategy
        /// </summary>
        public T GetPrimary()
        {
            return _primary;
        }

        /// <summary>
        /// List of all keyed strategies, with their key
        /// </summary>
        public IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies()
        {
            return _keyedStrategies.SelectMany(x => x.Value.Select(y => new KeyValuePair<object, T>(x.Key, y)));
        }

        /// <summary>
        /// Get all strategies for a given key (incl. ImportKey.Any)
        /// </summary>
        /// <param name="key">key</param>
        public IEnumerable<T> GetKeyedStrategies(object key)
        {
            var keyMatched = _keyedStrategies.TryGetValue(key, out var keys);
            var anyMatched = _keyedStrategies.TryGetValue(ImportKey.Any, out var anyKeys);

            return (keyMatched, anyMatched) switch
            {
                (true, true) => keys.Concat(anyKeys),
                (true, _) => keys,
                (_, true) => anyKeys,
                _ => [],
            };
        }

        /// <summary>
        /// Get main keyed strategy for a given key (fallbacks to ImportKey.Any)
        /// </summary>
        /// <param name="key">key</param>
        public T GetKeyedStrategy(object key)
        {
            return _keyedStrategies.TryGetValue(key, out var keys) 
                || _keyedStrategies.TryGetValue(ImportKey.Any, out keys)
                ? keys[^1]
                : null;
        }

        /// <summary>
        /// Clone the collection
        /// </summary>
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
            _keyedStrategies = ImmutableHashTree<object, ImmutableArray<T>>.Empty;

            foreach (var strategy in strategies)
            {
                strategy.Dispose();
            }

            foreach (var keyedStrategy in keyedStrategies.SelectMany(x => x.Value))
            {
                keyedStrategy.Dispose();
            }
        }
    }
}
