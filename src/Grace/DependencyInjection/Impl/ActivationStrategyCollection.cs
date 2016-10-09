using System;
using System.Collections.Generic;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class ActivationStrategyCollection<T> : IActivationStrategyCollection<T> where T : class, IActivationStrategy
    {
        private readonly Type _exportType;
        private T _primary;
        private bool _hasPriorities;
        private bool _hasConditions;
        private ImmutableArray<T> _strategies = ImmutableArray<T>.Empty;
        private ImmutableHashTree<object, T> _keyedStrategies = ImmutableHashTree<object, T>.Empty;

        public ActivationStrategyCollection(Type exportType)
        {
            _exportType = exportType;
        }

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

        public ImmutableArray<T> GetStrategies()
        {
            return _strategies;
        }

        public T GetPrimary()
        {
            return _primary;
        }

        public IEnumerable<KeyValuePair<object, T>> GetKeyedStrategies()
        {
            return _keyedStrategies;
        }

        public T GetKeyedStrategy(object key)
        {
            return _keyedStrategies.GetValueOrDefault(key);
        }

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
    }
}
